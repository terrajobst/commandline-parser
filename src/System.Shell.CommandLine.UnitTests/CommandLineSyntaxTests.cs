using System;

using Xunit;

namespace System.Shell.UnitTests
{
    public class CommandLineSyntaxTests
    {
        [Fact]
        public void CommandLineSyntax_DetectsNonExistingQualifier()
        {
            var commandLine = "-e -d";

            var exception = Assert.Throws<CommandLineSyntaxException>(() =>
            {
                var syntax = new CommandLineSyntax(commandLine);

                var exists = false;
                syntax.DefineQualifier("e|exists", ref exists, "Some qualifier");
                
                syntax.Validate();
            });

            Assert.Equal("invalid qualifier -d", exception.Message);
        }

        [Fact]
        public void CommandLineSyntax_DetectsDuplicateQualifiers()
        {
            var commandLine = "-a -b -a";

            var exception = Assert.Throws<CommandLineSyntaxException>(() =>
            {
                var syntax = new CommandLineSyntax(commandLine);

                var arg1 = false;
                var arg2 = false;
                syntax.DefineQualifier("a|arg 1", ref arg1, string.Empty);
                syntax.DefineQualifier("b|arg 2", ref arg2, string.Empty);

                syntax.Validate();
            });

            Assert.Equal("qualifier -a is specified multiple times", exception.Message);
        }

        [Fact]
        public void CommandLineSyntax_ParsesArrayQualifiers()
        {
            var commandLine = "-a x -b -a y";

            var syntax = new CommandLineSyntax(commandLine);

            var arg1 = new string[0];
            var arg2 = false;
            syntax.DefineQualifier("a|arg 1", ref arg1, string.Empty);
            syntax.DefineQualifier("b|arg 2", ref arg2, string.Empty);
            syntax.Validate();

            var expected1 = new[] {"x", "y"};
            var actual1 = arg1;
            Assert.Equal(expected1, actual1);

            var expected2 = true;
            var actual2 = arg2;
            Assert.Equal(expected2, actual2);
        }

        [Fact]
        public void CommandLineSyntax_ParsesArrayParameters()
        {
            var commandLine = "source1.cs source2.cs";

            var syntax = new CommandLineSyntax(commandLine);

            var sources = new string[0];
            syntax.DefineParameter("sources", ref sources, string.Empty);
            syntax.Validate();

            var expected = new[] { "source1.cs", "source2.cs" };
            var actual = sources;
            Assert.Equal(expected, actual);
        }
    }
}
