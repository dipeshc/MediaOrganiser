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

		private static String _OutputFileType = "mp4";
		public String OutputFileType
		{
			get
			{
				return _OutputFileType;
			}
		}

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
				return ShowDetailsAdditional.EpisodeName;
			}
		}

		public DateTime? AiredDate
		{
			get
			{
				return ShowDetailsAdditional.AiredDate;
			}
		}

		public String Overview
		{
			get
			{
				return ShowDetailsAdditional.Overview;
			}
		}

		public String TVNetwork
		{
			get
			{
				return ShowDetailsAdditional.TVNetwork;
			}
		}

		public IEnumerable<IFile> Artworks
		{
			get
			{
				return ShowDetailsAdditional.Artworks;
			}
		}

		public Boolean RequiresConversion
		{
			get
			{
				return (MediaFile.Extension.ToLower() != "."+OutputFileType);
			}
		}

		public Show(IFile MediaFile)
		{
			this.MediaFile = MediaFile;
		}

		public Boolean ExtractDetails(Boolean DoExhaustiveExtraction=true)
		{
			// 1) Try getting directly from file meta data.
			if(ShowDetailsAtomic.HasDetails || ShowDetailsAtomic.ExtractDetails(MediaFile))
			{
				ShowDetailsBasic = ShowDetailsAtomic;
				ShowDetailsAdditional = ShowDetailsAtomic;
			}

			// 2) If unable to get from file metadata then try getting from file name.
			if(!HasDetails && (ShowDetailsRegex.HasDetails || ShowDetailsRegex.ExtractDetails(MediaFile.Name)))
			{
				ShowDetailsBasic = ShowDetailsRegex;
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

		public String CleanFileName
		{
			get
			{
				String ShowFileName = "";
	
				// Add show name.
				ShowFileName +=  ShowDetailsBasic.ShowName + " - ";
	
				// Add season number.
				if(ShowDetailsBasic.SeasonNumber!=null)
				{
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
	
				return ShowFileName;
			}
		}

		public void Convert()
		{
			// Create file for converted version of show.
			IFile ConvertedMediaFile = new File(MediaFile.FullNameWithoutExtension + "." + OutputFileType);

			// Convert show.
			Convertor.Convertor.ConvertForiPad(MediaFile, ConvertedMediaFile);

			// Delete old file and assign the new converted file to the show.
			IFile OldFile = MediaFile;
			MediaFile = ConvertedMediaFile;
			OldFile.Delete();
		}
	}
}

