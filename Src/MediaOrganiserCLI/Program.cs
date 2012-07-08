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
            [OptionList("i", "input", Required=true, Separator=',', HelpText = "The Input folders from which media will be found. Comma seperated for mutiple folders.")]
            public IList<String> Inputs {get; set;}

            [Option("o", "outputs", HelpText = "The Output folder from which organised media will be put.")]
            public String Output { get; set; }

			[OptionList("e", "excluded", Separator=',', HelpText = "Media found in the Input folders that match shows found in the Excluded folders will not be organised. Comma seperated for mutiple folders.")]
			public IList<String> Excludes { get; set; }

			[Option("a", "addToiTunes", Required=false, HelpText = "Adds the converted media to iTunes.")]
			public Boolean AddToiTunes { get; set; }

			[Option("e", "excludeiTunesMedia", Required=false, HelpText = "Adds the iTunes media location to the list of excludes.")]
            public Boolean ExcludeiTunesMedia { get; set; }

			[Option("c", "clean", Required=false, HelpText = "Removes any temporary files and/or cache.")]
			public Boolean Clean { get; set; }

			public IEnumerable<IPath> InputPaths
			{
				get
				{
					return Inputs.Select(aPath => new Path(aPath));
				}
			}

			public IDirectory OutputDirectory
			{
				get
				{
					return Output==null?null:new Directory(Output);
				}
			}

			public IEnumerable<IPath> ExcludedPaths
			{
				get
				{
					return Excludes==null?null:Excludes.Select(aPath => new Path(aPath));
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
			var a = new CommandLineParserSettings(Console.Error);
            CommandLineParser Parser = new CommandLineParser(a);
            if (!Parser.ParseArguments(Args, Options))
			{
				// Exit 1.
                Environment.Exit(1);
			}

			Organiser Organiser = new Organiser(Options.InputPaths, Options.OutputDirectory, Options.ExcludedPaths, Options.Clean, Options.AddToiTunes, Options.ExcludeiTunesMedia);
			Organiser.Organise();

			// Exit 0.
			Environment.Exit(0);
		}
	}
}
