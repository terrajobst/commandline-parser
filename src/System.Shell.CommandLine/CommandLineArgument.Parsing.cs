using System;
using System.Collections.Generic;
using System.Text;

namespace System.Shell
{
    internal partial class CommandLineArgument
    {
        internal static IReadOnlyList<string> Split(string commandLine)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            var pos = 0;

            while (pos < commandLine.Length)
            {
                var c = commandLine[pos];

                if (c == ' ')
                {
                    AddArgument(result, sb);
                }
                else if (c == '"')
                {
                    var openingQuote = pos++;

                    while (pos < commandLine.Length)
                    {
                        if (commandLine[pos] == '"')
                        {
                            // Check if quote is escaped
                            if (pos + 1 < commandLine.Length && commandLine[pos + 1] == '"')
                                pos++;
                            else
                                break;
                        }

                        // Check for backslash quote
                        if (commandLine[pos] == '\\' && pos + 1 < commandLine.Length && commandLine[pos + 1] == '"')
                            pos++;

                        sb.Append(commandLine[pos]);
                        pos++;
                    }

                    if (pos >= commandLine.Length)
                        throw new Exception(String.Format("Unmatched quote at position {0}", openingQuote));
                }
                else
                {
                    sb.Append(commandLine[pos]);
                }

                pos++;
            }

            AddArgument(result, sb);

            return result.ToArray();
        }

        private static void AddArgument(ICollection<string> receiver, StringBuilder sb)
        {
            if (sb.Length == 0)
                return;

            var token = sb.ToString().Trim();
            if (token.Length > 0)
                receiver.Add(token);

            sb.Clear();
        }

        public static IReadOnlyList<CommandLineArgument> Parse(string commandLine)
        {
            var arguments = Split(commandLine);
            return Parse(arguments);
        }

        public static IReadOnlyList<CommandLineArgument> Parse(IEnumerable<string> commandLineArguments)
        {
            var result = new List<CommandLineArgument>();
            var hasSeenDashDash = false;

            foreach (var argument in commandLineArguments)
            {
                if (!hasSeenDashDash && argument == "--")
                {
                    hasSeenDashDash = true;
                    continue;
                }

                string modifier;
                string remainder;
                bool isModifier;

                if (!hasSeenDashDash)
                {
                    isModifier = TryExtractModifer(argument, out modifier, out remainder);
                }
                else
                {
                    modifier = null;
                    remainder = argument;
                    isModifier = false;
                }

                if (!isModifier)
                    remainder = argument;

                string key;
                string value;
                if (!isModifier || !TrySplitKeyValue(remainder, out key, out value))
                {
                    key = remainder;
                    value = null;
                }

                var parsedArgument = new CommandLineArgument(modifier, key, value);
                result.Add(parsedArgument);
            }

            for (var i = result.Count - 1; i >= 0; i--)
            {
                if (NeedsSingleLetterExpansion(result[i]))
                    ExpandSingleLetterArguments(result, i);
            }

            return result.ToArray();
        }

        private static bool NeedsSingleLetterExpansion(CommandLineArgument argument)
        {
            return argument.IsQualifier &&
                   argument.Modifier == "-" &&
                   argument.Name.Length > 1;
        }

        private static void ExpandSingleLetterArguments(List<CommandLineArgument> receiver, int index)
        {
            var qualifiers = receiver[index].Name;
            receiver.RemoveAt(index);

            foreach (var c in qualifiers)
            {
                var singleLetterName = char.ToString(c);
                var expandedArgument = new CommandLineArgument("-", singleLetterName, null);
                receiver.Insert(index, expandedArgument);
                index++;
            }
        }

        private static bool TryExtractModifer(string text, out string modifier, out string remainder)
        {
            return TryExtractPrefix(text, "--", out modifier, out remainder) ||
                   TryExtractPrefix(text, "-", out modifier, out remainder) ||
                   TryExtractPrefix(text, "/", out modifier, out remainder);
        }

        private static bool TryExtractPrefix(string text, string prefix, out string prefixResult, out string remainder)
        {
            if (text.StartsWith(prefix))
            {
                remainder = text.Substring(prefix.Length);
                prefixResult = prefix;
                return true;
            }

            remainder = null;
            prefixResult = null;
            return false;
        }

        private static bool TrySplitKeyValue(string text, out string key, out string value)
        {
            return TrySplitKeyValue(text, ':', out key, out value) ||
                   TrySplitKeyValue(text, '=', out key, out value);
        }

        private static bool TrySplitKeyValue(string text, char separator, out string key, out string value)
        {
            var i = text.IndexOf(separator);
            if (i >= 0)
            {
                key = text.Substring(0, i);
                value = text.Substring(i + 1);
                return true;
            }

            key = null;
            value = null;
            return false;
        }
    }
}