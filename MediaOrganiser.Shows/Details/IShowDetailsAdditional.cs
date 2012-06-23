using System;

namespace MediaOrganiser.Shows.Details
{
	public interface IShowDetailsAdditional
	{
		String EpisodeName { get; set; }
		DateTime? AiredDate { get; set; }
		String Overview { get; set; }
		String TVNetwork { get; set; }
		Boolean HasExtractedDetails { get; }
	}
}

