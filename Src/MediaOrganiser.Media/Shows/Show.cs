using System;
using System.Linq;
using System.Files;
using System.Collections.Generic;
using MediaOrganiser.Convertor;
using MediaOrganiser.Media.Shows.Details;

namespace MediaOrganiser.Media.Shows
{
	public class Show : IShow
	{
		public IFile MediaFile { get; set; }

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

		public IEnumerable<IFile> Artworks
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

		public Show(IFile MediaFile)
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

		public IFile OrganisedMediaFile
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
					ShowFilePath = FileSystem.PathCombine(ShowFilePath, String.Format("Season {0}", ShowDetailsBasic.SeasonNumber));
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
				ShowFilePath = ShowFilePath.Trim(FileSystem.GetInvalidPathChars());
				ShowFileName = ShowFileName.Trim(FileSystem.GetInvalidFileNameChars());
				
				// Return the full file path.
				return new File(FileSystem.PathCombine(ShowFilePath, ShowFileName));
			}
		}

		public void Convert()
		{
			// Create file for converted version of show.
			IFile ConvertedMediaFile = new File(MediaFile.FullNameWithoutExtension + ".converted." + OrganisedFileType);

			// Convert show.
			Convertor.Convertor.ConvertForRetina(MediaFile, ConvertedMediaFile);

			// Delete old file and assign the new converted file to the show.
			IFile OldFile = MediaFile;
			MediaFile = ConvertedMediaFile;
			OldFile.Delete();
		}
	}
}

