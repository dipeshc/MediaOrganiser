using System;
using System.Files.Interfaces;
using MediaOrganiser.Media;
using System.Collections.Generic;

namespace MediaOrganiser.Organisers
{
	public interface IOrganiser
	{
		IDirectory OutputDirectory { get; }
		Boolean AddToiTunes { get; }
		Boolean ForceConversion { get; }
		IDirectory WorkingDirectory { get; }

		void Organise(IEnumerable<IMedia> Medias);
		void Organise(IMedia Media);
	}
}

