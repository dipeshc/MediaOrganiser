using System;
using System.Linq;
using System.Files;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MediaOrganiser;
using MediaOrganiser.Media;
using MediaOrganiser.Finders;
using System.Threading;
using System.Threading.Tasks;
using System.Logging;

namespace MediaOrganiserCLI
{
	class Program
	{
		private static Int32 DaemonModeThreadWaitTimeInMilliseconds = 5000;

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

			[Option("d", "daemon", Required=false, HelpText = "Runs the application as a daemon task.")]
            public Boolean Daemon { get; set; }

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
					List<IPath> _ExcludedPaths = new List<IPath>();
					if(OutputDirectory!=null)
					{
						_ExcludedPaths.Add(new Path(OutputDirectory.FullName));
					}
					if(ExcludeiTunesMedia)
					{
						_ExcludedPaths.Add(new Path(Apple.iTunes.Properties.RootMediaDirectory.FullName));
					}
					return _ExcludedPaths;
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

			// Create organiser and finders.
			Organiser Organiser = new Organiser(Options.OutputDirectory, Options.Clean, Options.AddToiTunes);
			IMediaFinder ShowFinder = new ShowFinder(Options.InputPaths, Options.ExcludedPaths);

			// Check if need to run as daemon or one off.
			if(Options.Daemon)
			{
				RunAsDaemon(Organiser, ShowFinder);
			}
			else
			{
				Organiser.Organise(ShowFinder.Scan());
			}

			// Exit 0.
			Environment.Exit(0);
		}

		private static void RunAsDaemon(Organiser Organiser, IMediaFinder MediaFinder)
		{
			// Create queue.
			Queue<IMedia> MediaToBeOrganised = new Queue<IMedia>();

			// Create new thread to do inital scan and setup watchers.
			(new Thread(() =>
			{
				MediaFinder.ScanAndWatch((Sender, Media) =>
				{
					Log.WriteLine("Enqueing {0}.", Media.MediaFile.FullName);
					MediaToBeOrganised.Enqueue(Media);
					Log.WriteLine("Enqueued {0}.", Media.MediaFile.FullName);
				});
			})
			{
				Name = "Scan and Watch Thread",
				Priority = ThreadPriority.BelowNormal
			}).Start();

			// Run forever and take of queue as required.
			while(true)
			{
				Log.WriteLine("Media queue length {0}.", MediaToBeOrganised.Count);
				while(MediaToBeOrganised.Count!=0)
				{
					IMedia Media = MediaToBeOrganised.Dequeue();
					Log.WriteLine("Organising {0}.", Media.MediaFile.FullName);
					Organiser.Organise(Media);
					Log.WriteLine("Organised {0}.", Media.MediaFile.FullName);

				}
				Log.WriteLine("Waiting...");
				Thread.Sleep(DaemonModeThreadWaitTimeInMilliseconds);
			}
		}
	}
}
