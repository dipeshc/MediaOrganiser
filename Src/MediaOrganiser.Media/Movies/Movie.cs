using System;
using System.Files;
using System.Media;
using AtomicParsley;
using MediaOrganiser.Convertor;

namespace MediaOrganiser.Media.Movies
{
	public class Movie : IMovie
	{
		public IFile MediaFile { get; set; }

		private static String _OrganisedFileType = "mp4";
		public String OrganisedFileType
		{
			get
			{
				return _OrganisedFileType;
			}
		}

		public Boolean HasDetails
		{
			get
			{
				return false;
			}
		}

		public Boolean HasFullDetails
		{
			get
			{
				return false;
			}
		}

		public String Name
		{
			get
			{
				// TODO.
				return "";
			}
		}

		public DateTime? Year
		{
			get
			{
				// TODO.
				return null;
			}
		}

		public Boolean RequiresConversion
		{
			get
			{
				return (MediaFile.Extension.ToLower() != "."+OrganisedFileType);
			}
		}

		public Movie(IFile MediaFile)
		{
			this.MediaFile = MediaFile;
		}

		public Boolean ExtractDetails(Boolean DoExhaustiveExtraction=true)
		{
			// TODO.
			return false;
		}

		public void SaveDetails()
		{
			// TODO.
			//AtomicParsley.AtomicParsley.SetDetails(MediaFile.FullName, ShowName, SeasonNumber, EpisodeNumber, EpisodeName, AiredDate, Overview, TVNetwork);
		}

		public IFile OrganisedMediaFile
		{
			get
			{
				// TODO.
				return null;
			}
		}

		public void Convert()
		{
			// Create file for converted version of show.
			IFile ConvertedMediaFile = new File(MediaFile.FullNameWithoutExtension + "." + OrganisedFileType);

			// Convert show.
			Convertor.Convertor.ConvertForiPad(MediaFile, ConvertedMediaFile);

			// Delete old file and assign the new converted file to the show.
			IFile OldFile = MediaFile;
			MediaFile = ConvertedMediaFile;
			OldFile.Delete();
		}
	}
}

