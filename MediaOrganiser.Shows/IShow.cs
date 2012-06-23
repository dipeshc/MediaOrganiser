using System;
using System.Files;
using MediaOrganiser.Shows.Details;

namespace MediaOrganiser.Shows
{
	public interface IShow
	{
		IShowDetailsBasic ShowDetailsBasic { get; }
		IShowDetailsAdditional ShowDetailsAdditional { get; }
		IFile ShowFile { get ; set; }
		Boolean HasBasicDetails { get; }
		Boolean HasAdditionalDetails { get; }
		Boolean ExtractBasicDetails();
		Boolean ExtractAdditionalDetails();
		void SaveDetails();
		String ShowFileName { get; }
	}
}

