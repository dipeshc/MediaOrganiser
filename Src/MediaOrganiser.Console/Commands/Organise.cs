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

		private string _inputPath = "";
		private DirectoryInfoBase _outputDirectory = null;
		private bool _exclude = false;
		private bool _strictSeason = false;
		private OrganiserConversionOptions _conversionOption = OrganiserConversionOptions.Default;

		public Organise()
		{
			IsCommand("organise", "Organises the media (TV Shows) into a uniform ouput which can be consumed.");
			HasRequiredOption("i|input=", "The Input path from which media will be found.", v => _inputPath = v);
			HasRequiredOption("o|output=", "The Output directory from which organised media will be put.", v => _outputDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(v));
			HasOption("x|exclude", "Excludes organised shows in output directory when searching the input path.", v => _exclude = v != null);
			HasOption("s|strictSeason", "Enforces a strict season number requirement. This will guarantee the output media contains season details.", v => _strictSeason = v != null);
			HasOption("c|conversion=", "Specifies if/when input media should be converted. Possibly values: \"default\", \"force\", or \"skip\". \"default\" only converts if required, \"force\" converts everything, and \"skip\" will skip conversion for anything that requires conversion.", v =>
			{
				if(v == null || v.ToLower() == "default")
				{
					_conversionOption = OrganiserConversionOptions.Default;
				}
				else if(v.ToLower() == "force")
				{
					_conversionOption = OrganiserConversionOptions.Force;
				}
				else if(v.ToLower() == "skip")
				{
					_conversionOption = OrganiserConversionOptions.Skip;
				}
				else
				{
					throw new ApplicationException("Invalid conversion option provided.");
				}
			});
		}

		public override int Run(string[] remainingArguments)
		{
			// Check inputPath exists.
			if(!_fileSystem.DirectoryInfo.FromDirectoryName(_inputPath).Exists && !_fileSystem.FileInfo.FromFileName(_inputPath).Exists)
			{
				throw new ApplicationException("Invalid input path provided.");
			}
			
			// Check output folder exists.
			if(!_outputDirectory.Exists)
			{
				throw new ApplicationException("Invalid ouput directory provided.");
			}
			
			// Setup paths.
			var inputPaths = new List<string>() { _inputPath };
			var excludedPaths = new List<string>();
			if(_exclude)
			{
				excludedPaths.Add(_outputDirectory.FullName);
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
				organiser.Organise(media, _outputDirectory, _conversionOption, _strictSeason);
			});

			// Return 0.
			return 0;
		}
	}
}

