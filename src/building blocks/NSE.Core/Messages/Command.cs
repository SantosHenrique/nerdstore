﻿using System;
using System.ComponentModel.DataAnnotations;

namespace NSE.Core.Messages
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public virtual bool Valido()
        {
            throw new NotImplementedException();
        }
    }
}
