using MediaOrganiser.Convertor;
using MediaOrganiser.Media.Shows.Details;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace MediaOrganiser.Media.Shows
{
	public class Show : IShow
	{
		private IFileSystem fileSystem = new FileSystem();

		private IShowDetailsBasic showDetailsBasic = null;
		private IShowDetailsAdditional showDetailsAdditional = null;

		private ShowDetailsRegex showDetailsRegex = new ShowDetailsRegex();
		private ShowDetailsTVDB showDetailsTVDB = new ShowDetailsTVDB();
		private ShowDetailsAtomic showDetailsAtomic = new ShowDetailsAtomic();

		private static string OrganisedFileType = "mp4";

		public FileInfoBase MediaFile { get; set; }

		public bool HasDetails { get { return (showDetailsBasic!=null); } }
		public bool HasFullDetails { get { return (showDetailsAdditional!=null); } }
		public string ShowName { get { return showDetailsBasic.ShowName; } }
		public int? SeasonNumber { get { return showDetailsBasic.SeasonNumber; } }
		public int? EpisodeNumber { get { return showDetailsBasic.EpisodeNumber; } }
		public string EpisodeName { get { return showDetailsAdditional==null?null:showDetailsAdditional.EpisodeName; } }
		public DateTime? AiredDate { get { return showDetailsAdditional==null?null:showDetailsAdditional.AiredDate; } }
		public string Overview { get { return showDetailsAdditional==null?null:showDetailsAdditional.Overview; } }
		public string TVNetwork { get { return showDetailsAdditional==null?null:showDetailsAdditional.TVNetwork; } }
		public IEnumerable<FileInfoBase> Artworks { get { return showDetailsAdditional==null?null:showDetailsAdditional.Artworks; } }
		public bool RequiresConversion { get { return (MediaFile.Extension.ToLower() != "."+OrganisedFileType); } }

		public Show(FileInfoBase mediaFile)
		{
			MediaFile = mediaFile;
		}

		public bool ExtractDetails(bool doExhaustiveExtraction=true)
		{
			// 1) Try getting from file name.
			if(showDetailsRegex.HasDetails || showDetailsRegex.ExtractDetails(MediaFile.Name))
			{
				showDetailsBasic = showDetailsRegex;
			}

			// 2) If DoExhaustiveExtraction set then try getting directly from file meta data.
			if(doExhaustiveExtraction && (showDetailsAtomic.HasDetails || showDetailsAtomic.ExtractDetails(MediaFile)))
			{
				showDetailsBasic = showDetailsAtomic;
				showDetailsAdditional = showDetailsAtomic;
			}

			// 3) If HasDetails and DoExhaustiveExtraction then try getting additional details from online.
			try
			{
				if(HasDetails && doExhaustiveExtraction && (showDetailsTVDB.HasDetails || showDetailsTVDB.ExtractDetails(showDetailsBasic)))
				{
					showDetailsBasic = showDetailsTVDB;
					showDetailsAdditional = showDetailsTVDB;
				}
			}
			catch
			{
			}

			return HasDetails;
		}

		public bool SaveDetails()
		{
			return AtomicParsley.AtomicParsley.SetDetails(MediaFile.FullName, ShowName, SeasonNumber, EpisodeNumber, EpisodeName, AiredDate, Overview, TVNetwork, Artworks==null?null:Artworks.Select(A=>A.FullName));
		}

		public FileInfoBase OrganisedMediaFile
		{
			get
			{
				// Initalise empty filename and filepath.
				var showFilePath = "";
				var showFileName = "";
				
				// Add show name.
				showFilePath += showDetailsBasic.ShowName;
				showFileName += showDetailsBasic.ShowName + " - ";
				
				// Add season number.
				if(showDetailsBasic.SeasonNumber!=null)
				{
					showFilePath = fileSystem.Path.Combine(showFilePath, String.Format("Season {0}", showDetailsBasic.SeasonNumber));
					showFileName += String.Format("S{0:D2}", showDetailsBasic.SeasonNumber); 
				}
				
				// Add episode number.
				showFileName += String.Format("E{0:D2}", showDetailsBasic.EpisodeNumber);
				
				// Add epsisode name.
				if(HasFullDetails && showDetailsAdditional.EpisodeName != null)
				{
					showFileName += " - "+showDetailsAdditional.EpisodeName;
				}
				
				// Add extension.
				showFileName += MediaFile.Extension;
				
				// Sanitise.
				showFilePath = showFilePath.Trim(fileSystem.Path.GetInvalidPathChars());
				showFileName = showFileName.Trim(fileSystem.Path.GetInvalidFileNameChars());
				
				// Return the full file path.
				return fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(showFilePath, showFileName));
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

