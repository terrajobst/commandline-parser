﻿using System;
using System.IO;
using System.Linq;

using Xunit;

namespace System.UnitTests
{
    public class ArgumentParserTests
    {
        [Fact]
        public void ArgumentParser_SplitsSimpleWords()
        {
            var text = "abc def ghi";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "def",
                "ghi"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_FoldsMultipleSpaces()
        {
            var text = "abc  def       ghi";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "def",
                "ghi"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_HandlesQuotes()
        {
            var text = "abc \"def  ghi\"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "def  ghi"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_TrimsLeadingWhitespaceInQuotes()
        {
            var text = "abc \" def\"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "def"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_TrimsTrailingWhitespaceInQuotes()
        {
            var text = "abc \"def \"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "def"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_DetectsUnterminatedQuote()
        {
            var text = "abc \"def";
            var exeption = Assert.Throws<Exception>(() => CommandLineArgument.Split(text));
            Assert.Equal("Unmatched quote at position 4", exeption.Message);
        }

        [Fact]
        public void ArgumentParser_EscapesNestedDoubleQuotes()
        {
            var text = "abc \"d\"\"ef\"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "d\"ef"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_EscapesNestedBackslashQuote()
        {
            var text = "abc \"d\\\"ef\"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abc",
                "d\"ef"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_FoldsTopLevelDoubleQuotes()
        {
            var text = "abc\"\"def";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "abcdef"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_DoesTokenizeLikeShell()
        {
            var text = "-out test parmeter1.cs -o:test \"parameter with space.cs\" \"p\\\"aram\" \"parameter with \"\".cs\" \"-v=value\" \"-q\"=value -q=\"value\"";
            var tokens = CommandLineArgument.Split(text);
            var expected = new[] {
                "-out",
                "test",
                "parmeter1.cs",
                "-o:test",
                "parameter with space.cs",
                "p\"aram",
                "parameter with \".cs",
                "-v=value",
                "-q=value",
                "-q=value"
            };

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void ArgumentParser_GetsArguments()
        {
            var text = "abc def ghi";
            var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
            var expected = new[] {
                "abc",
                "def",
                "ghi",
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentParser_GetsOptions()
        {
            var text = "-a /b --c";
            var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
            var expected = new[] {
                "-a",
                "/b",
                "--c"
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentParser_GetsOptionArguments()
        {
            var text = "-a:va /b=vb --c vc";
            var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
            var expected = new[] {
                "-a:va",
                "/b:vb",
                "--c",
                "vc"
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentParser_ExpandsSingleLetterArguments()
        {
            var text = "-xdf";
            var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
            var expected = new[] {
                "-x",
                "-d",
                "-f"
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentParser_ExpandsSingleLetterArguments_UnlessUsingSlash()
        {
            var text = "/xdf";
            var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
            var expected = new[] {
                "/xdf"
            };

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ArgumentParser_ExpandsReponseFiles()
        {
            var responseFile = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(responseFile, new[] {
                    "-xdf",
                    "--out:out.exe",
                    "-r",
                    @"C:\Reference Assemblies\system.dll",
                });

                var text = "--before @\"" + responseFile + "\" --after";
                var actual = CommandLineArgument.Parse(text).Select(a => a.ToString());
                var expected = new[] {
                    "--before",
                    "-x",
                    "-d",
                    "-f",
                    "--out:out.exe",
                    "-r",
                    @"C:\Reference Assemblies\system.dll",
                    "--after"
                };

                Assert.Equal(expected, actual);
            }
            finally
            {
                File.Delete(responseFile);
            }
        }

        [Fact]
        public void ArgumentParser_ExpandsReponseFiles_UnlessItDoesntExist()
        {
            var responseFile = @"C:\non\existing\response_file.txt";
            var text = "--before @\"" + responseFile + "\" --after";
            var exception = Assert.Throws<CommandLineSyntaxException>(() => CommandLineArgument.Parse(text));
            var expected = string.Format("Response file '{0}' doesn't exist.", responseFile);
            var actual = exception.Message;
            
            Assert.Equal(expected, actual);
        }
    }
}