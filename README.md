# .NET Command Line Parser

This project is about providing an awesome command line parser for .NET.

Goals:

 - Lightweight
 - Very little configuration needed
 - Help strings
 - Support for multiple commands, like version control tools

## Syntax

### Single Letter Options

Single character options are delimited by a single dash, e.g.

    .\tool.exe -x -d -f

They can be *bundled* together, such as

    .\tool.exe -xdf

You can also use a slash, e.g.

    .\tool.exe /x /d /f

However, slashes don't support bundling. For example, the following isn't
recognized:

    # This is not equivalent to -xdf
    .\tool.exe /xdf

### Keyword Options

Keyword based options, also known as long-named options, are delimited by two
dashes, such as:

    .\tool.exe --verbose

Alternatively, you can use a slash:

    .\tool.exe /verbose

Using two dashes avoids any ambiguity with bundled forms -- which is why
slashes don't support bundling.

### Option Arguments

Both, the single letter form, as well as the long forms, support arguments.
Arguments must be separated by either a space, an equal sign or a colon:

    # All three forms are identical:
    .\tool.exe /out result.exe
    .\tool.exe /out=result.exe
    .\tool.exe /out:result.exe

Multiple spaces are allowed as well:

    .\tool.exe /out  result.exe
    .\tool.exe /out =   result.exe
    .\tool.exe /out : result.exe

### Parameters

Parameters, sometimes also called non-option arguments, can be anywhere in the
input:

    # Both forms equivalent:
    .\tool.exe input1.ext input2.ext -o result.ext
    .\tool.exe input1.ext -o result.ext input2.ext

### Commands

Very often, command line tools have multiple commands which have independent
syntaxes. Good example are version control tools, e.g.

    .\tool.exe pull origin --all
    .\tool.exe commit -m 'Message'

Sometimes having a default make sense. For example, if the tool originally only
had a single command but now has to support more commands you may want to use
a default command to avoid breaking existing customers. In other cases it may
just more convenient to use a default because one command is way more popular
than others.

For example, let's assume that `commit` would be the default command. In that
case both forms are equivalent:

    # Equivalent if 'commit' is the default command
    .\tool.exe commit -m 'Message'
    .\tool.exe -m 'Message'

Sometimes it's handy to think about how you can improve performance.

### Missing

- Make declaring qualifiers simpler by having a simple name that is pipe
  separated
- Support for arrays in options, e.g. -s first.cs -s second.cs
- Support for default commands
- Support response files using @
- Make -? and --help an intrinsic and remove it from the list
- Consider making all qualifiers optional and all parameters required
- Consider making everything option and rely on the consumer to validate
  requirements
- Consider custom syntax schemes
- Support undocumented switches

### Code

- Extract help generation from CommandLineSyntax
- Review argument checks
- Add tests
- Add comments & documentation

### Custom Syntax Schemes

As an alternative to defining qualifiers and parameters as required, we could
specify the usage as a string:

    syntax.DefineUsage("[(--catalog:v | (--datasource:v --database:v) )] [--metadata:v] <repository>");
    syntax.DefineUsage("[(--catalog:v | (--datasource:v --database:v) )] <source-repository> <target-repository>");

We'd parse that string to a representation like the following. We could walk the
the tree to determine which usage is "the best" match and provide validation,
similar to C# overload resolution.

                          Sequence
                             │
                      ┌──────┴──────┐
                      │             │
                  Optional      Optional
                      │             │
                Alternative   --metadata:v
                      │
               ┌──────┴──────┐
               │             │
         --catalog:v      Sequence
                             │
                      ┌──────┴──────┐
                      │             │
               --datasource:v  --database:v

## Existing Conventions

* [Unix History][Unix-History]
* [POSIX Conventions][POSIX-Conventions]
* [GNU Standards for Command Line Interfaces][GNU]
* [GNU List of standard option names][GNU-Options]

[Unix-History]: http://catb.org/~esr/writings/taoup/html/ch10s05.html
[POSIX-Conventions]: http://www.cs.unicam.it/piergallini/home/materiale/gc/java/essential/attributes/_posix.html
[GNU]: http://www.gnu.org/prep/standards/html_node/Command_002dLine-Interfaces.html
[GNU-Options]: http://www.gnu.org/prep/standards/html_node/Option-Table.html#Option-Table
