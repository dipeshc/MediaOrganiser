using System;
using System.Files.Interfaces;

namespace MediaOrganiser.Media
{
	public interface IMedia
	{
		IFile MediaFile { get ; set; }
		IFile OrganisedMediaFile { get; }

		Boolean ExtractDetails(Boolean GetFullDetails=true);
		Boolean HasDetails { get; }
		Boolean HasFullDetails { get; }
		void SaveDetails();

		Boolean RequiresConversion { get; }
		void Convert();
	}
}

