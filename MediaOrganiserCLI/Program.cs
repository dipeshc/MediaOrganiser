using System;
using System.Linq;
using System.Files;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MediaOrganiser;

namespace MediaOrganiserCLI
{
	class Program
	{
		private sealed class Options : CommandLineOptionsBase
        {
            [OptionList("i", "input", Required=true, Separator=',', HelpText = "The Input folders from which shows will be found. Comma seperated for mutiple folders.")]
            public IList<String> Inputs {get; set;}

            [Option("o", "outputs", Required = true, HelpText = "The Output folder from which organised shows will be put.")]
            public String Output { get; set; }

			[OptionList("e", "excluded", Separator=',', HelpText = "Shows found in the Input folders that match shows found in the Excluded folders will not be organised. Comma seperated for mutiple folders.")]
			public IList<String> Excludes { get; set; }

			public IEnumerable<IDirectory> InputDirectories
			{
				get
				{
					return Inputs.Select(Path => new Directory(Path));
				}
			}

			public IDirectory OutputDirectory
			{
				get
				{
					return new Directory(Output);
				}
			}

			public IEnumerable<IDirectory> ExcludedDirectories
			{
				get
				{
					return Excludes.Select(Path => new Directory(Path));
				}
			}

			[HelpOption]
            public string GetUsage()
            {
                HelpText Help = new HelpText {
					Heading = "MediaOrganiserCLI",
					Copyright = new CopyrightInfo("Dipesh Chauhan", DateTime.Now.Year),
                	AdditionalNewLineAfterOption = true,
					AddDashesToOption = true,
					MaximumDisplayWidth = Console.LargestWindowWidth
				};
               	HandleParsingErrorsInHelp(Help);
				Help.AddPreOptionsLine("Usage: MediaOrganiserCLI -i <input folder path>,<another input folder path> -o <output folder path>");
				Help.AddPreOptionsLine("Usage: MediaOrganiserCLI --inputs <input folder path>,<another input folder path> --output <output folder path>");
				Help.AddPreOptionsLine("Usage: MediaOrganiserCLI -i <input folder path>,<another input folder path> -o <output folder path> -e <excluded folder path>,<another excluded folder path>");
				Help.AddPreOptionsLine("Usage: MediaOrganiserCLI --inputs <input folder path>,<another input folder path> --output <output folder path> --Excludes <excluded folder path>,<another excluded folder path>");
				Help.AddOptions(this);
                return Help;
            }

            private void HandleParsingErrorsInHelp(HelpText Help)
            {
                if (this.LastPostParsingState.Errors.Count > 0)
                {
                    var Errors = Help.RenderParsingErrorsText(this, 2);
                    if (!string.IsNullOrEmpty(Errors))
                    {
                        Help.AddPreOptionsLine(String.Concat(Environment.NewLine, "ERROR(S):"));
                        Help.AddPreOptionsLine(Errors);
                    }
                }
            }
        }

		public static void Main(string[] Args)
		{
			// Parse out options.
			Options Options = new Options();
            CommandLineParser Parser = new CommandLineParser(new CommandLineParserSettings(Console.Error));
            if (!Parser.ParseArguments(Args, Options))
			{
				// Exit 1.
                Environment.Exit(1);
			}
		
			Organiser Organiser = new Organiser(Options.InputDirectories, Options.OutputDirectory, Options.ExcludedDirectories);
			Organiser.Organise();

			// Exit 0.
			Environment.Exit(0);
		}
	}
}
