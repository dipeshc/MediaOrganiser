using System;
using System.Files;
using MediaOrganiser.Media;
using MediaOrganiser.Media.Shows.Details;

namespace MediaOrganiser.Media.Shows
{
	public interface IShow : IMedia, IShowDetailsBasic, IShowDetailsAdditional
	{
	}
}

