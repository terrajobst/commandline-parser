using System;

namespace System
{
    public partial class CommandLineSyntax
    {
        // Qualifiers

        public void DefineQualifier(string name, ref string value, string help)
        {
            DefineQualifier(name, ref value, v => v, help);
        }

        public void DefineQualifier(string name, ref bool value, string help)
        {
            DefineQualifier(name, ref value, bool.Parse, help);
        }

        public void DefineQualifier(string name, ref int value, string help)
        {
            DefineQualifier(name, ref value, int.Parse, help);
        }

        // Qualifier arrays

        public void DefineQualifier(string name, ref string[] value, string help)
        {
            DefineQualifier(name, ref value, v => v, help);
        }

        // Parameters

        public void DefineParameter(string name, ref string value, string help)
        {
            DefineParameter(name, ref value, v => v, help);
        }

        public void DefineParameter(string name, ref bool value, string help)
        {
            DefineParameter(name, ref value, bool.Parse, help);
        }

        public void DefineParameter(string name, ref int value, string help)
        {
            DefineParameter(name, ref value, int.Parse, help);
        }

        // Parameter arrays

        public void DefineParameter(string name, ref string[] value, string help)
        {
            DefineParameter(name, ref value, v => v, help);
        }
    }
}