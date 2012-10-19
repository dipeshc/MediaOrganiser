using System;

namespace MediaOrganiser.Media.Movies.Details
{
	public interface IMovieDetailsBasic
	{
		String Name { get; }
		DateTime? Year { get; }
	}
}

