using MediaOrganiser.Media.Shows.Details;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using TagLib;

namespace MediaOrganiser.Core
{
	public class ShowDetailsMetaDataTags : IShowDetailsAdditional
	{
		private File _showFileTag;
		public string ShowName { get; private set; }
		public int? SeasonNumber { get; private set; }
		public int? EpisodeNumber { get; private set; }
		public string EpisodeName { get; private set; }
		public DateTime? AiredDate { get; private set; }
		public string Overview { get; private set; }
		public string TVNetwork { get; private set; }
		public IEnumerable<FileInfoBase> Artworks { get; private set; }
		public bool HasDetails { get; private set; }

		public ShowDetailsMetaDataTags(FileInfoBase showFile)
		{
			_showFileTag = File.Create(showFile.FullName);
		}

		public bool ExtractDetails()
		{
			var tag = (TagLib.Mpeg4.AppleTag) _showFileTag.GetTag(TagTypes.Apple);

			ShowName = tag.GetText("tvsh").FirstOrDefault();
			SeasonNumber = Convert.ToInt32(tag.GetText("TVSeasonNum").FirstOrDefault());
			SeasonNumber = Convert.ToInt32(tag.GetText("TVEpisodeNum").FirstOrDefault());
			EpisodeName = tag.Title;
			AiredDate = new DateTime((int) tag.Year, 0, 0, 0, 0, 0, DateTimeKind.Unspecified);
			Overview = tag.GetText("ldes").FirstOrDefault();
			TVNetwork = tag.GetText("TVNetwork").FirstOrDefault();

			return true;
		}
	}
}

