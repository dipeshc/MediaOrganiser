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

			[Option("x", "excludeiTunesMedia", Required=false, HelpText = "Adds the iTunes media location to the list of excludes.")]
            public Boolean ExcludeiTunesMedia { get; set; }

			[Option("w", "watcherMode", Required=false, HelpText = "Runs the application in watcher mode. The application will not exit, instead it wil continue to watch the input directories.")]
            public Boolean WatcherMode { get; set; }

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
					if(Excludes==null)
					{
						return new List<IPath>();
					}
					return Excludes.Select(aPath => new Path(aPath));
				}
			}

			[HelpOption]
            public string GetUsage()
            {
                HelpText Help = new HelpText
				{
					Heading = "MediaOrganiserCLI",
					Copyright = new CopyrightInfo("Dipesh Chauhan", DateTime.Now.Year),
                	AdditionalNewLineAfterOption = true,
					AddDashesToOption = true,
					MaximumDisplayWidth = Console.LargestWindowWidth
				};
               	HandleParsingErrorsInHelp(Help);
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

			// Create media organiser.
			MediaOrganiser.MediaOrganiser MediaOrganiser = new MediaOrganiser.MediaOrganiser(Options.InputPaths, Options.ExcludedPaths, Options.OutputDirectory, Options.AddToiTunes, Options.ExcludeiTunesMedia, Options.Clean);

			// Check if need to run as daemon or one off.
			if(Options.WatcherMode)
			{
				MediaOrganiser.ExecuteInWatcherMode();
			}
			else
			{
				MediaOrganiser.Execute();
			}

			// Exit 0.
			Environment.Exit(0);
		}
	}
}
