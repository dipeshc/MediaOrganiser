using System;

namespace MediaOrganiser.Shows.Details
{
	public interface IShowDetailsBasic
	{
		String ShowName { get; set; }
		Int32? SeasonNumber { get; set; }
		Int32? EpisodeNumber { get; set; }
		Boolean HasExtractedDetails { get; }
	}
}

