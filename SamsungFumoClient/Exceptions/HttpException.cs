using System;

namespace SamsungFumoClient.Exceptions
{
    public class HttpException : Exception
    {
        public int ErrorCode { get; }
        public string? Description { get; }

        public HttpException(int errorCode, string? description = null)
            : base(description == null ? errorCode.ToString() 
                : $"{errorCode.ToString()}: {description}")
        {
            ErrorCode = errorCode;
            Description = description;
        }
    }
}