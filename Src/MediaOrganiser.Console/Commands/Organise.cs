using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Logger;
using ManyConsole;
using NDesk.Options;
using MediaOrganiser.Console.Finders;
using MediaOrganiser.Console.Organisers;

namespace MediaOrganiser.Console
{
	public class Organise : ConsoleCommand
	{
		private static IFileSystem _fileSystem = new FileSystem();

		string inputPath = "";
		DirectoryInfoBase outputDirectory = null;
		bool forceConversion = false;
		bool exclude = false;
		bool strictSeason = false;

		public Organise()
		{
			IsCommand("organise", "Organises the media (TV Shows) into a uniform ouput which can be consumed.");
			HasRequiredOption("i|input=", "The Input path from which media will be found.", v => inputPath = v);
			HasRequiredOption("o|output=", "The Output directory from which organised media will be put.", v => outputDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(v));
			HasOption("f|forceConversion", "Forces the conversion of all input media, even if the media is already in the correct format.", v => forceConversion = v != null);
			HasOption("x|exclude", "Excludes organised shows in output directory when searching the input path.", v => exclude = v != null);
			HasOption("s|strictSeason", "Enforces a strict season number requirement. This will guarantee the output media contains season details.", v => strictSeason = v != null);
		}

		public override int Run(string[] remainingArguments)
		{
			// Check inputPath exists.
			if(!_fileSystem.DirectoryInfo.FromDirectoryName(inputPath).Exists && !_fileSystem.FileInfo.FromFileName(inputPath).Exists)
			{
				throw new ApplicationException("Invalid input path provided.");
			}
			
			// Check output folder exists.
			if(!outputDirectory.Exists)
			{
				throw new ApplicationException("Invalid ouput directory provided.");
			}
			
			// Setup paths.
			var inputPaths = new List<string>() { inputPath };
			var excludedPaths = new List<string>();
			if(exclude)
			{
				excludedPaths.Add(outputDirectory.FullName);
			}

			// Find shows and movies.
			var showFinder = new ShowFinder(inputPaths, excludedPaths);
			var showsToOrganise = showFinder.Scan();
			var movieFinder = new MovieFinder(inputPaths, excludedPaths.Union(showsToOrganise.Select(show => show.MediaFile.FullName)));
			var moviesToOrganise = movieFinder.Scan();
			var mediaToOrganise = showsToOrganise.Union(moviesToOrganise).ToList();

			// Log what is going to be organised.
			Logger.Log().StdOut.WriteLine("Organising {0} files: ", mediaToOrganise.Count);
			mediaToOrganise.ForEach(media =>
			{
				Logger.Log().StdOut.WriteLine("\tOrganising: {0}", media.MediaFile.Name);
			});

			// Organise.
			var organiser = new Organiser();
			mediaToOrganise.ForEach(media =>
			{
				organiser.Organise(media, outputDirectory, forceConversion, strictSeason);
			});

			// Return 0.
			return 0;
		}
	}
}

