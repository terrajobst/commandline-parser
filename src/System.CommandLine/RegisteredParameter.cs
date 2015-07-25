using System;

namespace System
{
    internal sealed class RegisteredParameter : RegisteredArgument
    {
        private readonly string _name;

        public RegisteredParameter(RegisteredCommand command, string name, string help)
            : base(command, help)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}