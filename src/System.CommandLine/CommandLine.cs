using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
    public static class CommandLine
    {
        public static string JoinArguments(IEnumerable<string> arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            return string.Join(" ", arguments.Select(EscapeArgument));
        }

        public static string EscapeArgument(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (!NeedsEscaping(text))
                return text;

            var sb = new StringBuilder();

            sb.Append("\"");

            // Append all characters. In case we find a quote
            // we'll escape it by doubling it.

            foreach (var c in text)
            {
                if (c == '"')
                    sb.Append("\"");

                sb.Append(c);
            }

            // If the text ends with backslash, we need to add a space.
            // Otherwise it will escape the quote.

            if (sb.Length > 0 && sb[sb.Length - 1] == '\\')
                sb.Append(" ");

            sb.Append("\"");

            return sb.ToString();
        }

        private static bool NeedsEscaping(string text)
        {
            return text.Any(c => c == '"' || c == ' ') ||
                   text.EndsWith("\\");
        }

        public static void Parse(IEnumerable<string> arguments, Action<CommandLineSyntax> syntaxAction)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            if (syntaxAction == null)
                throw new ArgumentNullException("syntaxAction");

            var syntax = new CommandLineSyntax(arguments);

            var showHelp = false;

            try
            {
                syntax.DefineOptionalQualifier("?|help", ref showHelp, "Shows this help page");
                syntaxAction(syntax);
            }
            catch (CommandLineSyntaxException ex)
            {
                ShowErrorAndExit(ex, -1);
            }

            if (showHelp)
                ShowHelpAndExit(syntax, 0);

            try
            {
                syntax.Validate();
            }
            catch (CommandLineSyntaxException ex)
            {
                ShowError(ex);
                ShowHelpAndExit(syntax, -1);
            }
        }

        private static void ShowError(CommandLineSyntaxException ex)
        {
            Console.WriteLine("error: {0}", ex.Message);
        }

        private static void ShowErrorAndExit(CommandLineSyntaxException ex, int exitCode)
        {
            ShowError(ex);
            Environment.Exit(exitCode);
        }

        private static void ShowHelpAndExit(CommandLineSyntax syntax, int exitCode)
        {
            var processPath = Environment.GetCommandLineArgs()[0];
            var processName = Path.GetFileNameWithoutExtension(processPath);
            var processNameLowerCased = processName.ToLowerInvariant();
            var helpText = syntax.GetHelpText(processNameLowerCased, Console.WindowWidth - 2);
            Console.WriteLine(helpText);
            Environment.Exit(exitCode);
        }
    }
}