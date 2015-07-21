using System;

namespace System
{
    internal sealed partial class CommandLineArgument
    {
        private readonly string _modifier;
        private readonly string _name;
        private readonly string _value;

        private bool _isMatched;

        private CommandLineArgument(string modifier, string name, string value)
        {
            _modifier = modifier;
            _name = name;
            _value = value;
        }

        public string Modifier
        {
            get { return _modifier; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        public bool IsQualifier
        {
            get { return !string.IsNullOrEmpty(_modifier); }
        }

        public bool IsSeparator
        {
            get { return Name == ":" || Name == "="; }
        }

        public bool HasValue
        {
            get { return !string.IsNullOrEmpty(_value); }
        }

        public bool IsMatched
        {
            get { return _isMatched; }
        }

        public void MarkMatched()
        {
            _isMatched = true;
        }

        public override string ToString()
        {
            return HasValue
                ? string.Format("{0}{1}:{2}", _modifier, _name, _value)
                : string.Format("{0}{1}", _modifier, _name);
        }
    }
}