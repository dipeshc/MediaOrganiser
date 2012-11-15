using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsAdditional
	{
		string EpisodeName { get; }
		DateTime? AiredDate { get; }
		string Overview { get; }
		string TVNetwork { get; }
		IEnumerable<FileInfoBase> Artworks { get; }
	}
}