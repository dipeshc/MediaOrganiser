using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsAdditional
	{
		String EpisodeName { get; }
		DateTime? AiredDate { get; }
		String Overview { get; }
		String TVNetwork { get; }
		IEnumerable<FileInfoBase> Artworks { get; }
	}
}