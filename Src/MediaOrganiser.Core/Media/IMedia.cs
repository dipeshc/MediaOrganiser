using System;
using System.IO.Abstractions;

namespace MediaOrganiser.Media
{
	public interface IMedia
	{
		FileInfoBase MediaFile { get ; set; }
		string OrganisedMediaFileOutputPath { get; }

		bool ExtractDetails(bool getFullDetails=true);
		bool HasDetails { get; }
		bool HasFullDetails { get; }
		bool SaveDetails();

		bool RequiresConversion { get; }
		bool Convert();
	}
}

