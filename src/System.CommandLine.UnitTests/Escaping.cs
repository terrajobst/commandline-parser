using System;

using Xunit;

namespace System.UnitTests
{
    public class Escaping
    {
        [Fact]
        public void Space_IsEscaped()
        {
            var argument = "C:\\Program Files";
            var expected = "\"C:\\Program Files\"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Quote_IsEscaped()
        {
            var argument = "C:\\Program \"Files\"";
            var expected = "\"C:\\Program \"\"Files\"\"\"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TrailingBackslash_IsEscaped()
        {
            var argument = "C:\\test\\";
            var expected = "\"C:\\test\\ \"";
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SimpleText_IsNotEscaped()
        {
            var argument = "C:\\foo.txt";
            var expected = argument;
            var actual = CommandLine.EscapeArgument(argument);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Join_HandlesEscaping()
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