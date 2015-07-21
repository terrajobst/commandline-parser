using System;

namespace System
{
    internal abstract class RegisteredArgument
    {
        private readonly RegisteredCommand _command;
        private readonly bool _isRequired;
        private readonly string _help;

        private bool _isMatched;

        protected RegisteredArgument(RegisteredCommand command, bool isRequired, string help)
        {
            _command = command;
            _isRequired = isRequired;
            _help = help;
        }

        public RegisteredCommand Command
        {
            get { return _command; }
        }

        public bool IsRequired
        {
            get { return _isRequired; }
        }

        public bool IsOptional
        {
            get { return !_isRequired; }
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