using System;

using Xunit;

namespace System.UnitTests
{
    public class EscapingTests
    {
        [Fact]
        public void Escaping_Space_IsEscaped()
        {
            var argument = "C:\\Program Files";
            var expected = "\"C:\\Program Files\"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Escaping_Quote_IsEscaped()
        {
            var argument = "C:\\Program \"Files\"";
            var expected = "\"C:\\Program \"\"Files\"\"\"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Escaping_TrailingBackslash_IsEscaped()
        {
            var argument = "C:\\test\\";
            var expected = "\"C:\\test\\ \"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Escaping_SimpleText_IsNotEscaped()
        {
            var argument = "C:\\foo.txt";
            var expected = argument;
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Escaping_Join_HandlesEscaping()
        {
            var arguments = new[]
            {
                "C:\\foo.txt",
                "C:\\Program Files",
                "-f"
            };

            var expected = "C:\\foo.txt \"C:\\Program Files\" -f";
            var actual = CommandLine.JoinArguments(arguments);

            Assert.Equal(expected, actual);
        }
    }
}