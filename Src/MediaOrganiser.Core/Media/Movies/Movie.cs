using MediaOrganiser.Convertor;
using MediaOrganiser.Media.Movies.Details;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace MediaOrganiser.Media.Movies
{
	public class Movie : IMovie
	{
		private IFileSystem _fileSystem = null;
		private static string OrganisedFileType = "mp4";

		private IMovieDetailsBasic _movieDetailsBasic;
		private MovieDetailsRegex _movieDetailsRegex = new MovieDetailsRegex();

		public FileInfoBase MediaFile { get; set; }
		public bool HasDetails { get; private set; }
		public bool HasFullDetails { get { return HasDetails; } }
		public bool RequiresConversion { get { return (MediaFile.Extension.ToLower() != "."+OrganisedFileType); } }

		public string Name { get { return _movieDetailsBasic.Name; } }
		public int Year { get { return _movieDetailsBasic.Year; } }

		public Movie(IFileSystem fileSystem, FileInfoBase mediaFile)
		{
			_fileSystem = fileSystem;
			MediaFile = mediaFile;
		}
		
		public bool ExtractDetails(bool doExhaustiveExtraction, bool strict)
		{
			// Try getting from file name.
			if(_movieDetailsRegex.HasDetails || _movieDetailsRegex.ExtractDetails(MediaFile.Name))
			{
				_movieDetailsBasic = _movieDetailsRegex;
			}
			return HasDetails;
		}
		
		public bool SaveDetails()
		{
			// Save.
			return AtomicParsley.AtomicParsley.SetMovieDetails(MediaFile.FullName, Name, Year);
		}
		
		public string OrganisedMediaFileOutputPath { get { return string.Format("{0} ({1}).{2}", Name, Year, MediaFile.Extension); } }
		
		public bool Convert()
		{
			// Create file for converted version of show.
			var fullNameWithoutExtension = _fileSystem.Path.Combine(MediaFile.Directory.FullName, _fileSystem.Path.GetFileNameWithoutExtension(MediaFile.FullName));
			var convertedMediaFile = _fileSystem.FileInfo.FromFileName(fullNameWithoutExtension + ".converted." + OrganisedFileType);
			
			// Convert show.
			if(!Convertor.Convertor.Convert(MediaFile, convertedMediaFile))
			{
				return false;
			}
			
			// Delete old file and assign the new converted file to the show.
			var oldFile = MediaFile;
			MediaFile = convertedMediaFile;
			oldFile.Delete();
			
			// Return true.
			return true;
		}
	}
}

