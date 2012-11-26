using System;

namespace MediaOrganiser.Media.Movies
{
	public interface IMovie : IMedia
	{
		string Name { get; }
		int Year { get; }
	}
}

