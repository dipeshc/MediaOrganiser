using System;

namespace MediaOrganiser.Media.Movies.Details
{
	public interface IMovieDetailsBasic
	{
		string Name { get; }
		int Year { get; }
	}
}