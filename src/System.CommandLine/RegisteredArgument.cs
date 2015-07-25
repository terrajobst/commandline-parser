using System;

namespace System
{
    internal abstract class RegisteredArgument
    {
        private readonly RegisteredCommand _command;
        private readonly string _help;

        private bool _isMatched;

        protected RegisteredArgument(RegisteredCommand command, string help)
        {
            _command = command;
            _help = help;
        }

        public RegisteredCommand Command
        {
            get { return _command; }
        }

        public string Help
        {
            get { return _help; }
        }

        public bool IsMatched
        {
            get { return _isMatched; }
        }

        public bool IsMissing
        {
            get { return !_isMatched; }
        }

        public void MarkMatched()
        {
            _isMatched = true;
        }
    }
}