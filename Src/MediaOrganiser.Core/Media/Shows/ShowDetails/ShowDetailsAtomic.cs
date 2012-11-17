using System;
using System.IO.Abstractions;
using System.Linq;
using System.Collections.Generic;
using MediaOrganiser.Media.AtomicParsley;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsAtomic : IShowDetailsBasic, IShowDetailsAdditional
	{
		public string ShowName { get; private set; }
		public int? SeasonNumber { get; private set; }
		public int? EpisodeNumber { get; private set; }
		public string EpisodeName { get; private set; }
		public DateTime? AiredDate { get; private set; }
		public string Overview { get; private set; }
		public string TVNetwork { get; private set; }
		public IEnumerable<FileInfoBase> Artworks { get; private set; }
		public bool HasDetails { get; private set; }

		public bool ExtractDetails(FileInfoBase showFile)
		{
			// Run AtomicParsley to get details.
			var details = new Dictionary<string, string>();
			if(!AtomicParsley.AtomicParsley.ExtractDetails(showFile.FullName, out details))
			{
				return false;
			}

			// Extract the details out of the output.
			if(!details.ContainsKey("tvsh") || !details.ContainsKey("tves"))
			{
				return false;
			}

			// Set default values.
			ShowName = details["tvsh"];
			SeasonNumber = null;
			EpisodeNumber = Convert.ToInt32(details["tves"]);
			EpisodeName = null;
			AiredDate = null;
			Overview = null;
			TVNetwork = null;
			Artworks = null;

			// Set optional values.
			if(details.ContainsKey("tvsn"))
			{
				SeasonNumber = Convert.ToInt32(details["tvsn"]);
			}
			if(details.ContainsKey("@nam"))
			{
				EpisodeName = details["@nam"];
			}
			if(details.ContainsKey("@day"))
			{
				AiredDate = DateTime.Parse(details["@day"]);
			}
			if(details.ContainsKey("@desc"))
			{
				Overview = details["desc"];
			}
			if(details.ContainsKey("tvnn"))
			{
				TVNetwork = details["tvnn"];
			}

			HasDetails = true;
			return true;
		}
	}
}

