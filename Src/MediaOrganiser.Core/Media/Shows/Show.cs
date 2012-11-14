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

		public FileInfoBase MediaFile { get; set; }

		private IShowDetailsBasic ShowDetailsBasic = null;
		private IShowDetailsAdditional ShowDetailsAdditional = null;

		private ShowDetailsRegex ShowDetailsRegex = new ShowDetailsRegex();
		private ShowDetailsTVDB ShowDetailsTVDB = new ShowDetailsTVDB();
		private ShowDetailsAtomic ShowDetailsAtomic = new ShowDetailsAtomic();

		private static String OrganisedFileType = "mp4";

		public Boolean HasDetails
		{
			get
			{
				return (ShowDetailsBasic!=null);
			}
		}

		public Boolean HasFullDetails
		{
			get
			{
				return (ShowDetailsAdditional!=null);
			}
		}

		public String ShowName
		{
			get
			{
				return ShowDetailsBasic.ShowName;
			}
		}

		public Int32? SeasonNumber
		{
			get
			{
				return ShowDetailsBasic.SeasonNumber;
			}
		}

		public Int32? EpisodeNumber
		{
			get
			{
				return ShowDetailsBasic.EpisodeNumber;
			}
		}

		public String EpisodeName
		{
			get
			{
				return ShowDetailsAdditional==null?null:ShowDetailsAdditional.EpisodeName;
			}
		}

		public DateTime? AiredDate
		{
			get
			{
				return ShowDetailsAdditional==null?null:ShowDetailsAdditional.AiredDate;
			}
		}

		public String Overview
		{
			get
			{
				return ShowDetailsAdditional==null?null:ShowDetailsAdditional.Overview;
			}
		}

		public String TVNetwork
		{
			get
			{
				return ShowDetailsAdditional==null?null:ShowDetailsAdditional.TVNetwork;
			}
		}

		public IEnumerable<FileInfoBase> Artworks
		{
			get
			{
				return ShowDetailsAdditional==null?null:ShowDetailsAdditional.Artworks;
			}
		}

		public Boolean RequiresConversion
		{
			get
			{
				return (MediaFile.Extension.ToLower() != "."+OrganisedFileType);
			}
		}

		public Show(FileInfoBase MediaFile)
		{
			this.MediaFile = MediaFile;
		}

		public Boolean ExtractDetails(Boolean DoExhaustiveExtraction=true)
		{
			// 1) Try getting from file name.
			if(ShowDetailsRegex.HasDetails || ShowDetailsRegex.ExtractDetails(MediaFile.Name))
			{
				ShowDetailsBasic = ShowDetailsRegex;
			}

			// 2) If DoExhaustiveExtraction set then try getting directly from file meta data.
			if(DoExhaustiveExtraction && (ShowDetailsAtomic.HasDetails || ShowDetailsAtomic.ExtractDetails(MediaFile)))
			{
				ShowDetailsBasic = ShowDetailsAtomic;
				ShowDetailsAdditional = ShowDetailsAtomic;
			}

			// 3) If HasDetails and DoExhaustiveExtraction then try getting additional details from online.
			try
			{
				if(HasDetails && DoExhaustiveExtraction && (ShowDetailsTVDB.HasDetails || ShowDetailsTVDB.ExtractDetails(ShowDetailsBasic)))
				{
					ShowDetailsBasic = ShowDetailsTVDB;
					ShowDetailsAdditional = ShowDetailsTVDB;
				}
			}
			catch
			{
			}

			return HasDetails;
		}

		public void SaveDetails()
		{
			AtomicParsley.AtomicParsley.SetDetails(MediaFile.FullName, ShowName, SeasonNumber, EpisodeNumber, EpisodeName, AiredDate, Overview, TVNetwork, Artworks==null?null:Artworks.Select(A=>A.FullName));
		}

		public FileInfoBase OrganisedMediaFile
		{
			get
			{
				String ShowFilePath = "";
				String ShowFileName = "";
				
				// Add show name.
				ShowFilePath += ShowDetailsBasic.ShowName;
				ShowFileName += ShowDetailsBasic.ShowName + " - ";
				
				// Add season number.
				if(ShowDetailsBasic.SeasonNumber!=null)
				{
					ShowFilePath = fileSystem.Path.Combine(ShowFilePath, String.Format("Season {0}", ShowDetailsBasic.SeasonNumber));
					ShowFileName += String.Format("S{0:D2}", ShowDetailsBasic.SeasonNumber); 
				}
				
				// Add episode number.
				ShowFileName += String.Format("E{0:D2}", ShowDetailsBasic.EpisodeNumber);
				
				// Add epsisode name.
				if(HasFullDetails && ShowDetailsAdditional.EpisodeName != null)
				{
					ShowFileName += " - "+ShowDetailsAdditional.EpisodeName;
				}
				
				// Add extension.
				ShowFileName += MediaFile.Extension;
				
				// Sanitise.
				ShowFilePath = ShowFilePath.Trim(fileSystem.Path.GetInvalidPathChars());
				ShowFileName = ShowFileName.Trim(fileSystem.Path.GetInvalidFileNameChars());
				
				// Return the full file path.
				return fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(ShowFilePath, ShowFileName));
			}
		}

		public void Convert()
		{
			// Create file for converted version of show.
			var fullNameWithoutExtension = fileSystem.Path.Combine(MediaFile.Directory.FullName, fileSystem.Path.GetFileNameWithoutExtension(MediaFile.FullName));
			var ConvertedMediaFile = fileSystem.FileInfo.FromFileName(fullNameWithoutExtension + ".converted." + OrganisedFileType);

			// Convert show.
			Convertor.Convertor.ConvertForRetina(MediaFile, ConvertedMediaFile);

			// Delete old file and assign the new converted file to the show.
			var OldFile = MediaFile;
			MediaFile = ConvertedMediaFile;
			OldFile.Delete();
		}
	}
}

