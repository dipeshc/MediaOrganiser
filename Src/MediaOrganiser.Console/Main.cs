using System;
using System.Linq;
using ManyConsole;
using NDesk.Options;

namespace MediaOrganiser.Console
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			// Console arguments.
			var help = false;
			var inputPath = "";
			var ouputFolder = "";
			var forceConversion = false;

			// Set options.
			var options = new OptionSet()
			{
				{"h|?|help", "Show help.", v => help = v != null},
				{"i|input=","The Input folders from which media will be found. Comma seperated for mutiple folders.", v => inputPath = v},
				{"o|output=", "The Output folder from which organised media will be put.",v => ouputFolder = v},
				{"f|?|forceConversion","Forces the conversion of all input media, even if the media is already in the correct format.", v => forceConversion = v != null}
			};

			// Parse args.
			var extra = options.Parse(args);
			if (extra.Any() || help)
			{
				OutputHelpAndExit(options);
			}

		}

		private static void OutputHelpAndExit(OptionSet options)
		{
			options.WriteOptionDescriptions(System.Console.Out);
			Environment.Exit(1);
		}
	}
}
