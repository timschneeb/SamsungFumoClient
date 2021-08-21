using System;

namespace SamsungFumoClient.Exceptions
{
    public class TransactionAbortedException : Exception
    {
        public TransactionAbortedException() 
            : base("Server has aborted the transaction and closed the session. " +
                           "No further messages must be sent using this session instance") {}
    }
}