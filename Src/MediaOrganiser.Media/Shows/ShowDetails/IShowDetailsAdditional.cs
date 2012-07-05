using System;
using System.Files;
using System.Collections.Generic;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsAdditional
	{
		String EpisodeName { get; }
		DateTime? AiredDate { get; }
		String Overview { get; }
		String TVNetwork { get; }
		IEnumerable<IFile> Artworks { get; }
	}
}

