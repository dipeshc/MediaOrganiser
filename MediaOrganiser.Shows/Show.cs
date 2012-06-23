using System;
using System.Files;
using MediaOrganiser.Shows.Details;

namespace MediaOrganiser.Shows
{
	public class Show : IShow
	{
		public IFile ShowFile { get; set; }

		private IShowDetailsBasic _ShowDetailsBasic = null;
		public IShowDetailsBasic ShowDetailsBasic
		{
			get
			{
				return _ShowDetailsBasic;
			}
		}

		private IShowDetailsAdditional _ShowDetailsAdditional = null;
		public IShowDetailsAdditional ShowDetailsAdditional
		{
			get
			{
				return _ShowDetailsAdditional;
			}
		}

		public Boolean HasBasicDetails
		{
			get
			{
				return (_ShowDetailsBasic!=null);
			}
		}

		public Boolean HasAdditionalDetails
		{
			get
			{
				return (_ShowDetailsAdditional!=null);
			}
		}

		private ShowDetailsBasicRegex ShowDetailsBasicRegex = new ShowDetailsBasicRegex();
		private ShowDetailsAdditionalTVDB ShowDetailsAdditionalTVDB = new ShowDetailsAdditionalTVDB();
		private ShowDetailsAtomic ShowDetailsAtomic = new ShowDetailsAtomic();


		public Show(IFile ShowFile)
		{
			this.ShowFile = ShowFile;
		}

		public Boolean ExtractBasicDetails()
		{
			// Try getting directly from ShowDetails.
			if(ShowDetailsAtomic.HasExtractedDetails || ShowDetailsAtomic.ExtractDetails(ShowFile))
			{
				_ShowDetailsBasic = ShowDetailsAtomic;
				_ShowDetailsAdditional = ShowDetailsAtomic;
				return true;
			}

			// Try getting from file name.
			if(ShowDetailsBasicRegex.HasExtractedDetails || ShowDetailsBasicRegex.ExtractDetails(ShowFile.Name))
			{
				_ShowDetailsBasic = ShowDetailsBasicRegex;
				return true;
			}

			return false;
		}

		public Boolean ExtractAdditionalDetails()
		{
			// Try getting directly from ShowDetails.
			if(ShowDetailsAtomic.HasExtractedDetails || ShowDetailsAtomic.ExtractDetails(ShowFile))
			{
				_ShowDetailsBasic = ShowDetailsAtomic;
				_ShowDetailsAdditional = ShowDetailsAtomic;
				return true;
			}

			// Try getting from online.
			if(ShowDetailsAdditionalTVDB.HasExtractedDetails || ShowDetailsAdditionalTVDB.ExtractDetails(ShowDetailsBasic))
			{
				_ShowDetailsAdditional = ShowDetailsAdditionalTVDB;
				return true;
			}

			return false;
		}

		public void SaveDetails()
		{
			if(HasAdditionalDetails)
			{
				AtomicParsley.AtomicParsley.SetDetails(ShowFile.FullName, _ShowDetailsBasic.ShowName, _ShowDetailsBasic.SeasonNumber, _ShowDetailsBasic.EpisodeNumber,
			                                       _ShowDetailsAdditional.EpisodeName, _ShowDetailsAdditional.AiredDate, _ShowDetailsAdditional.Overview, _ShowDetailsAdditional.TVNetwork);
			}
			else
			{
				AtomicParsley.AtomicParsley.SetDetails(ShowFile.FullName, _ShowDetailsBasic.ShowName, _ShowDetailsBasic.SeasonNumber, _ShowDetailsBasic.EpisodeNumber, null, null, null, null);
			}
		}

		public String ShowFileName
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
				if(HasAdditionalDetails && ShowDetailsAdditional.EpisodeName != null)
				{
					ShowFileName += " - "+ShowDetailsAdditional.EpisodeName;
				}
	
				// Add extension.
				ShowFileName += ShowFile.Extension;
	
				return ShowFileName;
			}
		}

		public override String ToString()
		{
			return ShowFile.FullName;
		}
	}
}

