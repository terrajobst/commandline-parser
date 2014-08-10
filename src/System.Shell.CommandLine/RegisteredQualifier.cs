using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace System.Shell
{
    internal sealed class RegisteredQualifier : RegisteredArgument
    {
        private readonly ReadOnlyCollection<string> _names;
        private readonly string _name;

        public RegisteredQualifier(RegisteredCommand command, IEnumerable<string> names, bool isRequired, string help)
            : base(command, isRequired, help)
        {
            _names = new ReadOnlyCollection<string>(names.ToArray());
            var firstLongName = _names.Where(n => n.Length > 1).FirstOrDefault();
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

        public bool HasMultipleNames
        {
            get { return _names.Skip(1).Any(); }
        }
    }
}