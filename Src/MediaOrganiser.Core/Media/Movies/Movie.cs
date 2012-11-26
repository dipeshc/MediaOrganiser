using MediaOrganiser.Convertor;
using MediaOrganiser.Media.Shows.Details;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace MediaOrganiser.Media.Movies
{
	public class Movie : IMovie
	{
		private IFileSystem fileSystem = new FileSystem();
		
		private static string OrganisedFileType = "mp4";
		
		public FileInfoBase MediaFile { get; set; }
		public bool HasDetails { get; private set; }
		public bool HasFullDetails { get { return HasDetails; } }

		public string Name { get; private set; }
		public int Year { get; private set; }

		public Movie(FileInfoBase mediaFile)
		{
			MediaFile = mediaFile;
		}
		
		public bool ExtractDetails(bool doExhaustiveExtraction=true)
		{
			return false;
		}
		
		public bool SaveDetails()
		{
			return false;
		}
		
		public string OrganisedMediaFileOutputPath
		{
			get
			{
				// Initalise empty filename and filepath.
				var movieFileName = "";
				
				// Return.
				return string.Format("{0}.{1}", movieFileName, MediaFile.Extension);
			}
		}
		
		public bool Convert()
		{
			// Create file for converted version of show.
			var fullNameWithoutExtension = fileSystem.Path.Combine(MediaFile.Directory.FullName, fileSystem.Path.GetFileNameWithoutExtension(MediaFile.FullName));
			var convertedMediaFile = fileSystem.FileInfo.FromFileName(fullNameWithoutExtension + ".converted." + OrganisedFileType);
			
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

