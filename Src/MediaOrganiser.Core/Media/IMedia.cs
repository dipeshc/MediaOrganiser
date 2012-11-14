using System;
using System.IO.Abstractions;

namespace MediaOrganiser.Media
{
	public interface IMedia
	{
		FileInfoBase MediaFile { get ; set; }
		FileInfoBase OrganisedMediaFile { get; }

		Boolean ExtractDetails(Boolean GetFullDetails=true);
		Boolean HasDetails { get; }
		Boolean HasFullDetails { get; }
		void SaveDetails();

		Boolean RequiresConversion { get; }
		void Convert();
	}
}

