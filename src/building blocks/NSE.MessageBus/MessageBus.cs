using EasyNetQ;
using NSE.Core.Messages.Integration;
using Polly;
using RabbitMQ.Client.Exceptions;
using System;
using System.Threading.Tasks;

namespace NSE.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private readonly string _connectionString;
        private IBus _bus;
        private IAdvancedBus _advancedBus;

        public IAdvancedBus _advanced => _bus?.Advanced;
        
        public bool IsConnected => _bus?.Advanced.IsConnected ?? false;

        public IAdvancedBus AdvancedBus => _bus?.Advanced;

        public MessageBus(string connectionString)
        {
            _connectionString = connectionString;
            TryConnect();
        }

        public void Publish<T>(T message) where T : IntegrationEvent
        {
            TryConnect();
            _bus.PubSub.Publish(message);
        }

        public async Task PublishAsync<T>(T message) where T : IntegrationEvent
        {
            await _bus.PubSub.PublishAsync(message);
        }

        public TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.Request<TRequest, TResponse>(request);
        }

        public async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return await _bus.Rpc.RequestAsync<TRequest, TResponse>(request);
        }

        public IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.Respond(responder);
        }

        public Task<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage
        {
            TryConnect();
            return _bus.Rpc.RespondAsync(responder);
        }

        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            TryConnect();
            _bus.PubSub.Subscribe(subscriptionId, onMessage);
        }

        public async Task SubscriveAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
        {
            await _bus.PubSub.SubscribeAsync(subscriptionId, onMessage);
        }

        private void TryConnect()
        {
            if (IsConnected) return;

            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            policy.Execute(() =>
            {
                _bus = RabbitHutch.CreateBus(_connectionString);
                _advancedBus = _bus.Advanced;
                _advancedBus.Disconnected += OnDisconnect;
            });
        }

        private void OnDisconnect(object s, EventArgs e)
        {
            var policy = Policy.Handle<EasyNetQException>()
                .Or<BrokerUnreachableException>()
                .RetryForever();

            policy.Execute(TryConnect);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }
    }

    public interface IMessageBus : IDisposable
    {
        bool IsConnected { get; }
        IAdvancedBus AdvancedBus { get; }
        void Publish<T>(T message) where T : IntegrationEvent;

        Task PublishAsync<T>(T message) where T : IntegrationEvent;

        void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class;

        Task SubscriveAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class;

        TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;

        Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;

        IDisposable Respond<TRequest, TResponse>(Func<TRequest, TResponse> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;

        Task<IDisposable> RespondAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> responder)
            where TRequest : IntegrationEvent
            where TResponse : ResponseMessage;

    }
}
