using System;

namespace System.Shell
{
    internal sealed class RegisteredCommand
    {
        private readonly string _name;
        private readonly string _help;

        public RegisteredCommand(string name, string help)
        {
            _name = name;
            _help = help;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Help
        {
            get { return _help; }
        }
    }
}