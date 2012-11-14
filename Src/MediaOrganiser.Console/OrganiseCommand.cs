using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using ManyConsole;
using NDesk.Options;
using MediaOrganiser.Console.Finders;
using MediaOrganiser.Organisers;

namespace MediaOrganiser.Console
{
	public class OrganiseCommand : ConsoleCommand
	{
		private static IFileSystem fileSystem = new FileSystem();

		string inputPath = "";
		DirectoryInfoBase outputDirectory = null;
		bool forceConversion = false;

		public OrganiseCommand()
		{
			IsCommand("organise", "Organises the media (TV Shows) into a uniform ouput which can be consumed.");
			HasRequiredOption("i|input=", "The Input folders from which media will be found.", v => inputPath = v);
			HasRequiredOption("o|output=", "The Output folder from which organised media will be put.", v => outputDirectory = fileSystem.DirectoryInfo.FromDirectoryName(v));
			HasOption("f|forceConversion", "Forces the conversion of all input media, even if the media is already in the correct format.", v => forceConversion = v != null);
		}

		public override int Run(string[] remainingArguments)
		{
			// Check inputPath exists.
			if(!fileSystem.DirectoryInfo.FromDirectoryName(inputPath).Exists && !fileSystem.FileInfo.FromFileName(inputPath).Exists)
			{
				throw new ApplicationException("Invalid input path provided.");
			}
			
			// Check output folder exists.
			if(!outputDirectory.Exists)
			{
				throw new ApplicationException("Invalid ouput directory provided.");
			}
			
			// Get files to organise.
			var showFinder = new ShowFinder(new List<string>() {inputPath}, new List<string>());
			var mediaToOrganise = showFinder.Scan().ToList();
			
			// Organise.
			var organiser = new Organiser();
			mediaToOrganise.ForEach(media =>
			{
				organiser.Organise(media, outputDirectory, forceConversion);
			});

			// Return 0.
			return 0;
		}
	}
}

