using System;
using System.Files;
using System.Collections.Generic;
using MediaOrganiser.Media;

namespace MediaOrganiser.Finders
{
	public delegate void MediaFoundEventHandler (IMediaFinder MediaFinder, IMedia Media);

	public interface IMediaFinder
	{
		IEnumerable<IPath> InputPaths { get; }
		IEnumerable<IPath> ExcludedPaths { get; }
		IEnumerable<IMedia> Scan();
		void ScanAndWatch(MediaFoundEventHandler Handler);
	}
}

