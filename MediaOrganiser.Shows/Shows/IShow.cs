using System;
using MediaOrganiser.Media.Shows.Details;

namespace MediaOrganiser.Media.Shows
{
	public interface IShow : IMedia, IShowDetailsBasic, IShowDetailsAdditional
	{
	}
}

