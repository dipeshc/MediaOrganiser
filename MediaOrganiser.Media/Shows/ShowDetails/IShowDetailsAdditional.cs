using System;
using System.Files;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsAdditional
	{
		String EpisodeName { get; }
		DateTime? AiredDate { get; }
		String Overview { get; }
		String TVNetwork { get; }
		IFile Artwork { get; }
	}
}

