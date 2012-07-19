using System;
using System.Files;
using MediaOrganiser.Media;
using System.Collections.Generic;

namespace MediaOrganiser.Organisers
{
	public interface IOrganiser
	{
		IDirectory OutputDirectory { get; }
		Boolean AddToiTunes { get; }
		IDirectory WorkingDirectory { get; }

		void Organise(IEnumerable<IMedia> Medias);
		void Organise(IMedia Media);
	}
}

