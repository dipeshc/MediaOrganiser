using System;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsBasic
	{
		String ShowName { get; }
		Int32? SeasonNumber { get; }
		Int32? EpisodeNumber { get; }
	}
}

