using System;

namespace MediaOrganiser.Media.Shows.Details
{
	public interface IShowDetailsBasic
	{
		string ShowName { get; }
		int? SeasonNumber { get; }
		int? EpisodeNumber { get; }
	}
}