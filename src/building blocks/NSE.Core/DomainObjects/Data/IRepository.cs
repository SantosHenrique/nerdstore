using System;

namespace NSE.Core.DomainObjects
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {

    }
}
