using MediaOrganiser.Media;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;

namespace MediaOrganiser.Console.Finders
{
	public delegate void MediaFoundEventHandler (IMediaFinder MediaFinder, IMedia Media);

	public interface IMediaFinder
	{
		IEnumerable<string> InputPaths { get; }
		IEnumerable<string> ExcludedPaths { get; }
		IEnumerable<IMedia> Scan();
	}
}

