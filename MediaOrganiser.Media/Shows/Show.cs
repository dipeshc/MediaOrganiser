using System;
using System.Files;
using MediaOrganiser.Convertor;
using MediaOrganiser.Media.Shows.Details;

namespace MediaOrganiser.Media.Shows
{
	public class Show : IShow
	{
		public IFile MediaFile { get; set; }

		private IShowDetailsBasic ShowDetailsBasic = null;
		private IShowDetailsAdditional ShowDetailsAdditional = null;

		private ShowDetailsBasicRegex ShowDetailsBasicRegex = new ShowDetailsBasicRegex();
		private ShowDetailsAdditionalTVDB ShowDetailsAdditionalTVDB = new ShowDetailsAdditionalTVDB();
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
			// Try getting directly from ShowDetails.
			if(ShowDetailsAtomic.HasDetails || ShowDetailsAtomic.ExtractDetails(MediaFile))
			{
				ShowDetailsBasic = ShowDetailsAtomic;
				ShowDetailsAdditional = ShowDetailsAtomic;
				return true;
			}

			// Try getting from file name.
			Boolean Extracted = false;
			if(ShowDetailsBasicRegex.HasDetails || ShowDetailsBasicRegex.ExtractDetails(MediaFile.Name))
			{
				ShowDetailsBasic = ShowDetailsBasicRegex;
				Extracted = true;
			}

			// Try getting from online if DoExhaustiveExtraction.
			try
			{
				if(DoExhaustiveExtraction && (ShowDetailsAdditionalTVDB.HasDetails || ShowDetailsAdditionalTVDB.ExtractDetails(ShowDetailsBasic)))
				{
					ShowDetailsAdditional = ShowDetailsAdditionalTVDB;
				}
			}
			catch
			{
			}

			return Extracted;
		}

		public void SaveDetails()
		{
			AtomicParsley.AtomicParsley.SetDetails(MediaFile.FullName, ShowName, SeasonNumber, EpisodeNumber, EpisodeName, AiredDate, Overview, TVNetwork);
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

