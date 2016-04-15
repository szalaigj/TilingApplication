using System;
using System.Runtime.Serialization;

namespace HierarchicalTilingApp.ArrayPartition
{
    [Serializable]
    public class NotEnoughBinsException : Exception
    {
        public NotEnoughBinsException ()
        {}

        public NotEnoughBinsException (string message) 
            : base(message)
        {}

        public NotEnoughBinsException (string message, Exception innerException)
            : base (message, innerException)
        {}

        protected NotEnoughBinsException(SerializationInfo info, StreamingContext context)
            : base (info, context)
        {}
    }
}
