using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Shell
{
    public sealed class CommandLineSyntax
    {
        private readonly IReadOnlyList<CommandLineArgument> _arguments;

        private readonly HashSet<string> _knownCommandNames = new HashSet<string>();
        private readonly HashSet<string> _knownQualifierNames = new HashSet<string>();
        private readonly HashSet<string> _knownParameterNames = new HashSet<string>();

        private readonly List<RegisteredCommand>  _registeredCommands = new List<RegisteredCommand>();
        private readonly List<RegisteredQualifier>  _registeredQualifiers = new List<RegisteredQualifier>();
        private readonly List<RegisteredParameter>  _registeredParameters = new List<RegisteredParameter>();

        private RegisteredCommand _definedCommand;
        private RegisteredCommand _parsedCommand;

        public CommandLineSyntax(string commandLine)
            : this(CommandLineArgument.Parse(commandLine))
        {
        }

        public CommandLineSyntax(IEnumerable<string> commandLineArguments)
            : this(CommandLineArgument.Parse(commandLineArguments))
        {
        }

        private CommandLineSyntax(IReadOnlyList<CommandLineArgument> arguments)
        {
            _arguments = arguments;
        }

        private RegisteredCommand RegisterCommand(string name, string help)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("You must specify a name", "name");

            if (!_knownCommandNames.Add(name))
                throw new ArgumentException(string.Format("Command '{0}' is already registered.", name));

            var registeredCommand = new RegisteredCommand(name, help);
            _registeredCommands.Add(registeredCommand);

            return registeredCommand;
        }

        private RegisteredQualifier RegisterQualifier(string name, bool isRequired, string help)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("You must specify a name", "name");

            var names = name.Split('|').Select(n => n.Trim());

            foreach (var alias in names)
            {
                if (!_knownQualifierNames.Add(alias))
                    throw new ArgumentException(string.Format("Qualifier '{0}' is already registered.", alias));
            }

            var registeredQualifier = new RegisteredQualifier(_definedCommand, names, isRequired, help);
            _registeredQualifiers.Add(registeredQualifier);

            return registeredQualifier;
        }

        private RegisteredParameter RegisterParameter(string name, bool isRequired, string help)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("You must specify a name", "name");

            if (!_knownParameterNames.Add(name))
                throw new ArgumentException(string.Format("Parameter '{0}' is already registered.", name));

            var registeredParameter = new RegisteredParameter(_definedCommand, name, isRequired, help);
            _registeredParameters.Add(registeredParameter);

            return registeredParameter;
        }

        private void EnsureNoParametersSeenForCurrentCommand()
        {
            if (_registeredParameters.Any(p => p.Command == _definedCommand))
                throw new InvalidOperationException("Qualifiers must be defined before any parameters.");
        }

        public void DefineCommand(string name, ref string command, string help)
        {
            DefineCommand(name, ref command, name, help);
        }

        public void DefineCommand<T>(string name, ref T command, T value, string help)
        {
            if (_registeredParameters.Any(c => c.Command == null))
                throw new InvalidOperationException("Cannot define commands if global parameters exist.");

            _definedCommand = RegisterCommand(name, help);

            if (_parsedCommand != null)
                return;

            var argument = _arguments.FirstOrDefault();
            if (argument == null || argument.IsQualifier || argument.IsSeparator)
                return;

            if (!string.Equals(argument.Name, name, StringComparison.Ordinal))
                return;

            argument.MarkMatched();
            command = value;
            _parsedCommand = _definedCommand;
        }

        public void DefineQualifier<T>(string name, ref T value, Func<string, T> valueConverter, string help)
        {
            value = DefineQualifier(name, value, valueConverter, help, true);
        }

        public void DefineQualifier<T>(string name, ref T[] value, Func<string, T> valueConverter, string help)
        {
            value = DefineQualifier(name, value, valueConverter, help, true);
        }

        public void DefineOptionalQualifier<T>(string name, ref T value, Func<string, T> valueConverter, string help)
        {
            value = DefineQualifier(name, value, valueConverter, help, false);
        }

        public void DefineOptionalQualifier<T>(string name, ref T[] value, Func<string, T> valueConverter, string help)
        {
            value = DefineQualifier(name, value, valueConverter, help, false);
        }

        private T DefineQualifier<T>(string name, T defaultValue, Func<string, T> valueConverter, string help, bool isRequired)
        {
            var arrayDefault = new[] { defaultValue };
            var result = DefineQualifier(name, arrayDefault, valueConverter, help, isRequired);
            if (result.Length > 1)
                throw new CommandLineSyntaxException(string.Format("qualifier {0} is specified multiple times", name));

            return result.Last();
        }

        private T[] DefineQualifier<T>(string name, T[] defaultValue, Func<string, T> valueConverter, string help, bool isRequired)
        {
            EnsureNoParametersSeenForCurrentCommand();

            var qualifier = RegisterQualifier(name, isRequired, help);

            if (_parsedCommand != _definedCommand)
                return defaultValue;

            var result = new List<T>();
            var argumentIndex = 0;
            var isBoolean = typeof(T) == typeof(bool);

            while (argumentIndex < _arguments.Count)
            {
                if (TryGetNextQualifier(ref argumentIndex, qualifier.Names))
                {
                    qualifier.MarkMatched();

                    string valueText;
                    if (TryGetValue(ref argumentIndex, isBoolean, out valueText))
                    {
                        var value = ParseQualifierValue(qualifier.Name, valueConverter, valueText);
                        result.Add(value);
                    }
                    else if (isBoolean)
                    {
                        var value = (T)(object)true;
                        result.Add(value);
                    }
                }

                argumentIndex++;
            }

            if (result.Any())
                return result.ToArray();

            return defaultValue;
        }

        public void DefineParameter<T>(string name, ref T value, Func<string, T> valueConverter, string help)
        {
            value = DefineParameter(name, value, valueConverter, help, true);
        }

        public void DefineParameter<T>(string name, ref T[] value, Func<string, T> valueConverter, string help)
        {
            value = DefineParameter(name, value, valueConverter, help, true);
        }

        public void DefineOptionalParameter<T>(string name, ref T value, Func<string, T> valueConverter, string help)
        {
            value = DefineParameter(name, value, valueConverter, help, false);
        }

        public void DefineOptionalParameter<T>(string name, ref T[] value, Func<string, T> valueConverter, string help)
        {
            value = DefineParameter(name, value, valueConverter, help, false);
        }

        private T DefineParameter<T>(string name, T defaultValue, Func<string, T> valueConverter, string help, bool isRequired)
        {
            var parameter = RegisterParameter(name, isRequired, help);

            if (_parsedCommand != _definedCommand)
                return defaultValue;

            string value;
            if (TryGetNextParameter(out value))
            {
                parameter.MarkMatched();
                return ParseParameterValue(name, valueConverter, value);
            }

            return defaultValue;
        }

        private T[] DefineParameter<T>(string name, T[] defaultValue, Func<string, T> valueConverter, string help, bool isRequired)
        {
            var parameter = RegisterParameter(name, isRequired, help);

            if (_parsedCommand != _definedCommand)
                return defaultValue;

            var result = new List<T>();

            while (true)
            {
                string value;
                if (!TryGetNextParameter(out value))
                    break;

                parameter.MarkMatched();
                var parsedValue = ParseParameterValue(name, valueConverter, value);
                result.Add(parsedValue);
            }

            if (result.Any())
                return result.ToArray();

            return defaultValue;
        }

        private bool TryGetNextQualifier(ref int argumentIndex, IReadOnlyCollection<string> names)
        {
            while (argumentIndex < _arguments.Count)
            {
                var a = _arguments[argumentIndex];

                if (a.IsQualifier)
                {
                    if (names.Any(n => string.Equals(a.Name, n, StringComparison.OrdinalIgnoreCase)))
                    {
                        a.MarkMatched();
                        return true;
                    }
                }

                argumentIndex++;
            }

            return false;
        }

        private bool TryGetValue(ref int argumentIndex, bool requiresSeparator, out string value)
        {
            var a = _arguments[argumentIndex];
            if (a.HasValue)
            {
                a.MarkMatched();
                value = a.Value;
                return true;
            }

            argumentIndex++;
            if (argumentIndex < _arguments.Count)
            {
                var b = _arguments[argumentIndex];
                if (!b.IsQualifier)
                {
                    // This might a value or a separator
                    if (!b.IsSeparator)
                    {
                        // If we require a separator we can't consume this.
                        if (requiresSeparator)
                            goto noResult;

                        b.MarkMatched();
                        value = b.Name;
                        return true;
                    }

                    // Skip separator
                    b.MarkMatched();
                    argumentIndex++;

                    if (argumentIndex < _arguments.Count)
                    {
                        var c = _arguments[argumentIndex];
                        if (!c.IsQualifier)
                        {
                            c.MarkMatched();
                            value = c.Name;
                            return true;
                        }
                    }
                }
            }

        noResult:
            value = null;
            return false;
        }

        private bool TryGetNextParameter(out string value)
        {
            foreach (var argument in _arguments)
            {
                if (argument.IsMatched || argument.IsQualifier || argument.IsSeparator)
                    continue;

                argument.MarkMatched();
                value = argument.Name;
                return true;
            }

            value = null;
            return false;
        }

        private static T ParseQualifierValue<T>(string name, Func<string, T> valueConverter, string valueText)
        {
            return ParseValue(name, true, valueConverter, valueText);
        }

        private static T ParseParameterValue<T>(string parameterName, Func<string, T> valueConverter, string valueText)
        {
            return ParseValue(parameterName, false, valueConverter, valueText);
        }

        private static T ParseValue<T>(string name, bool isQualifier, Func<string, T> valueConverter, string valueText)
        {
            try
            {
                return valueConverter(valueText);
            }
            catch (ArgumentException ex)
            {
                throw CreateSyntaxException(name, isQualifier, ex);
            }
            catch (FormatException ex)
            {
                throw CreateSyntaxException(name, isQualifier, ex);
            }
        }

        private static CommandLineSyntaxException CreateSyntaxException(string name, bool isQualifier, Exception ex)
        {
            var displayName = isQualifier ? string.Format("--{0}", name) : string.Format("<{0}>", name);
            var message = string.Format("cannot parse value for {0}: {1}", displayName, ex.Message);
            return new CommandLineSyntaxException(message);
        }

        public void Validate()
        {
            // Check whether we have a command

            if (_parsedCommand == null && _registeredCommands.Any())
            {
                var command = _arguments.FirstOrDefault();
                if (command != null && !command.IsQualifier && !command.IsSeparator)
                    throw new CommandLineSyntaxException(string.Format("unknown command '{0}'", command.Name));

                throw new CommandLineSyntaxException("missing command");
            }

            // Search for invalid qualifiers & extra parameter

            foreach (var argument in _arguments)
            {
                if (argument.IsMatched)
                    continue;

                if (argument.IsQualifier)
                {
                    var message = string.Format("invalid qualifier {0}{1}", argument.Modifier, argument.Name);
                    throw new CommandLineSyntaxException(message);
                }
                else
                {
                    var message = string.Format("extra parameter '{0}'", argument.Name);
                    throw new CommandLineSyntaxException(message);
                }
            }

            // Search for required qualifiers

            foreach (var qualifier in GetQualifiers(_parsedCommand))
            {
                if (qualifier.IsRequired && qualifier.IsMissing)
                    throw new CommandLineSyntaxException(string.Format("required qualifier '{0}' not specified", qualifier.Name));
            }

            // Search for required parameters

            foreach (var parameter in GetParameters(_parsedCommand))
            {
                if (parameter.IsRequired && parameter.IsMissing)
                    throw new CommandLineSyntaxException(string.Format("required parameter '{0}' not specified", parameter.Name));
            }
        }

        private IEnumerable<RegisteredQualifier> GetQualifiers(RegisteredCommand command)
        {
            return _registeredQualifiers.Where(q => q.Command == null || q.Command == command);
        }

        private IEnumerable<RegisteredParameter> GetParameters(RegisteredCommand command)
        {
            return _registeredParameters.Where(p => p.Command == command);
        }

        public string GetHelpText(string name)
        {
            return GetHelpText(name, int.MaxValue);
        }

        public string GetHelpText(string name, int maxWidth)
        {
            return _parsedCommand != null || !_registeredCommands.Any()
                        ? GetCommandHelpText(name, _parsedCommand, maxWidth)
                        : GetGlobalHelpText(name, maxWidth);
        }

        private string GetGlobalHelpText(string name, int maxWidth)
        {
            var sb = new StringBuilder();

            WriteSyntax(sb, name, GetGlobalSyntax(), maxWidth);

            sb.AppendLine();

            var maxColumnWidth = _registeredCommands.Select(c => c.Name.Length)
                                                    .DefaultIfEmpty()
                                                    .Max();

            var helpLineIndent = maxColumnWidth + 8;

            var maxHelpWidth = maxWidth - helpLineIndent;
            if (maxHelpWidth < 0)
                maxHelpWidth = maxWidth;

            sb.AppendLine("Available commands:");
            sb.AppendLine();

            foreach (var command in _registeredCommands)
            {
                var start = sb.Length;

                sb.Append(' ', 4);
                sb.Append(command.Name);

                var current = sb.Length - start;
                var filler = helpLineIndent - current;

                sb.Append(' ', filler);

                WriteHelp(sb, command.Help, helpLineIndent, maxHelpWidth);
            }

            return sb.ToString();
        }

        private string GetCommandHelpText(string name, RegisteredCommand command, int maxWidth)
        {
            var sb = new StringBuilder();

            // Syntax

            WriteSyntax(sb, name, GetCommandSyntax(command), maxWidth);

            // Parameters & Qualifiers

            if (GetParameterNames(command).Any() || GetQualifierNames(command).Any())
                sb.AppendLine();

            var maxColumnWidth = GetParameterNames(command).Concat(GetQualifierNames(command))
                                                           .Select(t => t.Length)
                                                           .DefaultIfEmpty()
                                                           .Max();
            var helpLineIndent = maxColumnWidth + 8;

            var maxHelpWidth = maxWidth - helpLineIndent;
            if (maxHelpWidth < 0)
                maxHelpWidth = maxWidth;

            var rows = GetParameterNames(command).Concat(GetQualifierNames(command))
                                                 .Zip(GetHelpTexts(command), (n, h) => new[] { n, h });

            foreach (var row in rows)
            {
                var columnName = row[0];
                var columnHelp = row[1];

                var start = sb.Length;

                sb.Append(' ', 4);
                sb.Append(columnName);

                var current = sb.Length - start;
                var filler = helpLineIndent - current;

                sb.Append(' ', filler);

                WriteHelp(sb, columnHelp, helpLineIndent, maxHelpWidth);
            }

            return sb.ToString();
        }

        private static void WriteHelp(StringBuilder sb, string help, int helpLineIndent, int maxHelpWidth)
        {
            var helpLines = WordWrapLines(SplitWords(help), maxHelpWidth);
            var isFirstHelpLine = true;

            foreach (var helpLine in helpLines)
            {
                if (isFirstHelpLine)
                    isFirstHelpLine = false;
                else
                    sb.Append(' ', helpLineIndent);

                sb.AppendLine(helpLine);
            }
        }

        private static void WriteSyntax(StringBuilder sb, string name, IEnumerable<string> syntax, int maxWidth)
        {
            sb.Append("usage: ");
            sb.Append(name);
            sb.Append(" ");

            var syntaxIndent = sb.Length;
            var syntaxWidth = syntaxIndent < maxWidth
                                ? maxWidth - syntaxIndent
                                : maxWidth;
            var syntaxTokens = syntax;
            var syntaxLines = WordWrapLines(syntaxTokens, syntaxWidth);
            var isFirst = true;

            foreach (var syntaxLine in syntaxLines)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(' ', syntaxIndent);

                sb.AppendLine(syntaxLine);
            }
        }

        private IEnumerable<string> GetGlobalSyntax()
        {
            foreach (var registeredQualifier in GetQualifiers(null))
                yield return GetQualifierSyntax(registeredQualifier);

            yield return "<command>";
            yield return "[<args>]";
        }

        private IEnumerable<string> GetCommandSyntax(RegisteredCommand command)
        {
            if (command != null)
                yield return command.Name;

            foreach (var registeredQualifier in GetQualifiers(command))
                yield return GetQualifierSyntax(registeredQualifier);

            if (!GetParameters(command).Any())
                yield break;

            yield return "[--]";

            foreach (var registeredParameter in GetParameters(command))
                yield return GetParameterSyntax(registeredParameter);
        }

        private static string GetQualifierSyntax(RegisteredQualifier registeredQualifier)
        {
            var sb = new StringBuilder();

            if (registeredQualifier.IsOptional)
                sb.Append("[");
            else if (registeredQualifier.HasMultipleNames)
                sb.Append("(");

            var isFirst = true;

            foreach (var name in registeredQualifier.Names)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append("|");

                var separator = name.Length == 1 ? "-" : "--";
                sb.Append(separator);
                sb.Append(name);
            }

            if (registeredQualifier.IsOptional)
                sb.Append("]");
            else if (registeredQualifier.HasMultipleNames)
                sb.Append(")");

            return sb.ToString();
        }

        private static string GetParameterSyntax(RegisteredParameter registeredParameter)
        {
            var sb = new StringBuilder();

            if (registeredParameter.IsOptional)
                sb.Append("[");

            sb.Append("<");
            sb.Append(registeredParameter.Name);
            sb.Append(">");

            if (registeredParameter.IsOptional)
                sb.Append("]");

            return sb.ToString();
        }

        private IEnumerable<string> GetParameterNames(RegisteredCommand command)
        {
            return GetParameters(command).Select(parameter => string.Concat("<", parameter.Name, ">"));
        }

        private IEnumerable<string> GetQualifierNames(RegisteredCommand command)
        {
            var sb = new StringBuilder();

            foreach (var registeredQualifier in GetQualifiers(command))
            {
                var isFirst = true;

                foreach (var name in registeredQualifier.Names)
                {
                    if (isFirst)
                        isFirst = false;
                    else
                        sb.Append(", ");

                    var separator = name.Length == 1 ? "-" : "--";
                    sb.Append(separator);
                    sb.Append(name);
                }

                yield return sb.ToString();
                sb.Clear();
            }
        }

        private IEnumerable<string> GetHelpTexts(RegisteredCommand command)
        {
            return GetParameters(command).Concat<RegisteredArgument>(GetQualifiers(command)).Select(a => a.Help);
        }

        private static IEnumerable<string> WordWrapLines(IEnumerable<string> tokens, int maxWidth)
        {
            var sb = new StringBuilder();

            foreach (var token in tokens)
            {
                var newLength = sb.Length == 0
                                  ? token.Length
                                  : sb.Length + 1 + token.Length;

                if (newLength > maxWidth)
                {
                    if (sb.Length == 0)
                    {
                        yield return token;
                        continue;
                    }

                    yield return sb.ToString();
                    sb.Clear();
                }

                if (sb.Length > 0)
                    sb.Append(" ");

                sb.Append(token);
            }

            if (sb.Length > 0)
                yield return sb.ToString();
        }

        private static IEnumerable<string> SplitWords(string text)
        {
            return text.Split(' ');
        }
    }
}