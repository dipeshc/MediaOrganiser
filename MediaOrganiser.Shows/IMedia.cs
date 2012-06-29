using System;
using System.Files;

namespace MediaOrganiser.Media
{
	public interface IMedia
	{
		IFile MediaFile { get ; set; }
		String OutputFileType { get; }
		String CleanFileName { get; }

		Boolean ExtractDetails(Boolean GetFullDetails=true);
		Boolean HasDetails { get; }
		Boolean HasFullDetails { get; }
		void SaveDetails();

		Boolean RequiresConversion { get; }
		void Convert();
	}
}

