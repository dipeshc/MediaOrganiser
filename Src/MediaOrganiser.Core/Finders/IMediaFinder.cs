using System;
using System.Files.Interfaces;
using System.Collections.Generic;
using MediaOrganiser.Media;

namespace MediaOrganiser.Finders
{
	public delegate void MediaFoundEventHandler (IMediaFinder MediaFinder, IMedia Media);

	public interface IMediaFinder
	{
		IEnumerable<String> FileExtensions { get; }
		IEnumerable<IPath> InputPaths { get; }
		IEnumerable<IPath> ExcludedPaths { get; }
		IEnumerable<IMedia> Scan();
		void ScanAndWatch(MediaFoundEventHandler Handler);
	}
}

