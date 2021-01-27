using System;

namespace Tf2Rebalance.CreateSummary.Converters
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException(string message)
            :base(message)
        {
            
        }
    }
    public class InputNotSupportedException : Exception
    {
        public InputNotSupportedException(string message)
            :base(message)
        {
            
        }
    }
}