using System;

namespace System
{
    public partial class CommandLineSyntax
    {
        private static readonly Func<string, string> StringParser = v => v;
        private static readonly Func<string, bool> BooleanParser = v => bool.Parse(v);
        private static readonly Func<string, int> Int32Parser = v => int.Parse(v, CultureInfo.InvariantCulture);

        // Qualifiers

        public void DefineQualifier(string name, ref string value, string help)
        {
            DefineQualifier(name, ref value, StringParser, help);
        }

        public void DefineQualifier(string name, ref bool value, string help)
        {
            DefineQualifier(name, ref value, BooleanParser, help);
        }

        public void DefineQualifier(string name, ref int value, string help)
        {
            DefineQualifier(name, ref value, Int32Parser, help);
        }

        // Qualifier arrays

        public void DefineQualifier(string name, ref string[] value, string help)
        {
            DefineQualifier(name, ref value, StringParser, help);
        }

        // Parameters

        public void DefineParameter(string name, ref string value, string help)
        {
            DefineParameter(name, ref value, StringParser, help);
        }

        public void DefineParameter(string name, ref bool value, string help)
        {
            DefineParameter(name, ref value, BooleanParser, help);
        }

        public void DefineParameter(string name, ref int value, string help)
        {
            DefineParameter(name, ref value, Int32Parser, help);
        }

        // Parameter arrays

        public void DefineParameter(string name, ref string[] value, string help)
        {
            DefineParameter(name, ref value, StringParser, help);
        }
    }
}