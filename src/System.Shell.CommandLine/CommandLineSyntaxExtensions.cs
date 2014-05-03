using System;

namespace System.Shell
{
    public static class CommandLineSyntaxExtensions
    {
        // Qualifiers

        public static void DefineQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref string value, string help)
        {
            syntax.DefineQualifier(singleLetterName, longName, ref value, v => v, help);
        }

        public static void DefineOptionalQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref string value, string help)
        {
            syntax.DefineOptionalQualifier(singleLetterName, longName, ref value, v => v, help);
        }

        public static void DefineQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref bool value, string help)
        {
            syntax.DefineQualifier(singleLetterName, longName, ref value, bool.Parse, help);
        }

        public static void DefineOptionalQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref bool value, string help)
        {
            syntax.DefineOptionalQualifier(singleLetterName, longName, ref value, bool.Parse, help);
        }

        public static void DefineQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref int value, string help)
        {
            syntax.DefineQualifier(singleLetterName, longName, ref value, int.Parse, help);
        }

        public static void DefineOptionalQualifier(this CommandLineSyntax syntax, string singleLetterName, string longName, ref int value, string help)
        {
            syntax.DefineOptionalQualifier(singleLetterName, longName, ref value, int.Parse, help);
        }       

        // Parameters

        public static void DefineParameter(this CommandLineSyntax syntax, string name, ref string value, string help)
        {
            syntax.DefineParameter(name, ref value, v => v, help);
        }

        public static void DefineOptionalParameter(this CommandLineSyntax syntax, string name, ref string value, string help)
        {
            syntax.DefineOptionalParameter(name, ref value, v => v, help);
        }

        public static void DefineParameter(this CommandLineSyntax syntax, string name, ref bool value, string help)
        {
            syntax.DefineParameter(name, ref value, bool.Parse, help);
        }

        public static void DefineOptionalParameter(this CommandLineSyntax syntax, string name, ref bool value, string help)
        {
            syntax.DefineOptionalParameter(name, ref value, bool.Parse, help);
        }

        public static void DefineParameter(this CommandLineSyntax syntax, string name, ref int value, string help)
        {
            syntax.DefineParameter(name, ref value, int.Parse, help);
        }

        public static void DefineOptionalParameter(this CommandLineSyntax syntax, string name, ref int value, string help)
        {
            syntax.DefineOptionalParameter(name, ref value, int.Parse, help);
        }
    }
}