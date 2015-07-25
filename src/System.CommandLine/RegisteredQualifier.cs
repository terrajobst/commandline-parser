using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System
{
    internal sealed class RegisteredQualifier : RegisteredArgument
    {
        private readonly ReadOnlyCollection<string> _names;
        private readonly string _name;

        public RegisteredQualifier(RegisteredCommand command, IEnumerable<string> names, string help)
            : base(command, help)
        {
            _names = new ReadOnlyCollection<string>(names.ToArray());
            var firstLongName = _names.FirstOrDefault(n => n.Length > 1);
            var firstName = _names.First();
            _name = firstLongName ?? firstName;
        }

        public string Name
        {
            get { return _name; }
        }

        public ReadOnlyCollection<string> Names
        {
            get { return _names; }
        }
    }
}