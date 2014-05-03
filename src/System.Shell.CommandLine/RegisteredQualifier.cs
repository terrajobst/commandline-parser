using System;

namespace System.Shell
{
    internal sealed class RegisteredQualifier : RegisteredArgument
    {
        private readonly string _singleLetterName;
        private readonly string _longName;

        public RegisteredQualifier(RegisteredCommand command, string singleLetterName, string longName, bool isRequired, string help)
            : base(command, isRequired, help)
        {
            _singleLetterName = singleLetterName;
            _longName = longName;
        }

        public string SingleLetterName
        {
            get { return _singleLetterName; }
        }

        public bool HasSingleLetterName
        {
            get { return !string.IsNullOrEmpty(_singleLetterName); }
        }

        public string LongName
        {
            get { return _longName; }
        }
    }
}