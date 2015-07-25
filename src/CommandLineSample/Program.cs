using System;

namespace CommandLineSample
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var isQuiet = false;
            var isDryRun = false;
            var useForce = false;

            var command = string.Empty;

            var message = string.Empty;
            var edit = false;
            var addSignoff = false;
            var pathSpec = string.Empty;

            var fetchTags = false;
            var fetchAll = false;
            var repository = string.Empty;
            var refSpec = string.Empty;

            var sourceFiles = new string[0];
            var references = new string[0];

            CommandLine.Parse(args, syntax =>
            {
                // Global qualifiers
                syntax.DefineQualifier("q|quiet", ref isQuiet, "do not print names of files removed");
                syntax.DefineQualifier("n|dry-run", ref isDryRun, "dry run");
                syntax.DefineQualifier("f|force", ref useForce, "force");

                // Commit
                syntax.DefineCommand("commit", ref command, "Record changes to the repository");
                syntax.DefineQualifier("m|message", ref message, "commit message");
                syntax.DefineQualifier("e|edit", ref edit, "force edit of commit");
                syntax.DefineQualifier("s|signoff", ref addSignoff, "add Signed-off-by:");
                syntax.DefineParameter("pathspec", ref pathSpec, "Path to a file");

                // Pull
                syntax.DefineCommand("pull", ref command, "Fetch from and integrate with another repository or a local branch");
                syntax.DefineQualifier("t|tags", ref fetchTags, "fetch all tags and associated objects");
                syntax.DefineQualifier("all", ref fetchAll, "fetch from all remotes");
                syntax.DefineParameter("repository", ref repository, "repository to pull from");
                syntax.DefineParameter("refspec", ref refSpec, "refspec to be pulled. Please note that this help text is quite extensive and should be completely read. Also note how it flows around quite nicely.");

                // Compile
                syntax.DefineCommand("compile", ref command, "Compile the sources using the C# compiler");
                syntax.DefineQualifier("r|reference", ref references, "Reference metadata from the specified assembly files");
                syntax.DefineParameter("file", ref sourceFiles, "C# source from the specified source files");
            });

            Console.WriteLine("command    = {0}", command);
            Console.WriteLine("isQuiet    = {0}", isQuiet);
            Console.WriteLine("isDryRun   = {0}", isDryRun);
            Console.WriteLine("useForce   = {0}", useForce);

            switch (command)
            {
                case null:
                case "":
                    break;
                case "commit":
                    Console.WriteLine("message    = {0}", message);
                    Console.WriteLine("edit       = {0}", edit);
                    Console.WriteLine("addSignoff = {0}", addSignoff);
                    break;
                case "pull":
                    Console.WriteLine("fetchTags  = {0}", fetchTags);
                    Console.WriteLine("fetchAll   = {0}", fetchAll);
                    Console.WriteLine("repository = {0}", repository);
                    Console.WriteLine("refSpec    = {0}", refSpec);
                    break;
                case "compile":
                    foreach (var value in references)
                        Console.WriteLine("reference  = {0}", value);
                    foreach (var value in sourceFiles)
                        Console.WriteLine("source     = {0}", value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
