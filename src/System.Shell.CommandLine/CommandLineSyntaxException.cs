using System;

namespace System.Shell
{
    public class CommandLineSyntaxException : Exception
    {
        public CommandLineSyntaxException()
        {
        }

        public CommandLineSyntaxException(string message)
            : base(message)
        {
        }

        public CommandLineSyntaxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}