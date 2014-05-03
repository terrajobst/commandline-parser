using System;

namespace System.Shell
{
    internal sealed class RegisteredParameter : RegisteredArgument
    {
        private readonly string _name;

        public RegisteredParameter(RegisteredCommand command, string name, bool isRequired, string help)
            : base(command, isRequired, help)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}