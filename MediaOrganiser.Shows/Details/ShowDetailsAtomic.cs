using System;
using System.Linq;
using System.Files;
using System.Collections.Generic;
using AtomicParsley;

namespace MediaOrganiser.Shows.Details
{
	public class ShowDetailsAtomic : IShowDetailsBasic, IShowDetailsAdditional
	{
		public String ShowName { get; set; }
		public Int32? SeasonNumber { get; set; }
		public Int32? EpisodeNumber { get; set; }
		public String EpisodeName { get; set; }
		public DateTime? AiredDate { get; set; }
		public String Overview { get; set; }
		public String TVNetwork { get; set; }

		private Boolean _HasExtractedDetails = false;
		public Boolean HasExtractedDetails
		{
			get
			{
				return _HasExtractedDetails;
			}
		}

		public Boolean ExtractDetails(IFile ShowFile)
		{
			// Run AtomicParsley to get details.
			Dictionary<String, String> Details = AtomicParsley.AtomicParsley.ExtractDetails(ShowFile.FullName);

			// Extract the details out of the output.
			if(!Details.ContainsKey("tvsh") || !Details.ContainsKey("tves"))
			{
				return false;
			}

			ShowName = Details["tvsh"];
			SeasonNumber = null;
			EpisodeNumber = Convert.ToInt32(Details["tves"]);
			EpisodeName = null;
			AiredDate = null;
			Overview = null;
			TVNetwork = null;

			if(Details.ContainsKey("tvsn"))
			{
				SeasonNumber = Convert.ToInt32(Details["tvsn"]);
			}
			if(Details.ContainsKey("@nam"))
			{
				EpisodeName = Details["@nam"];
			}
			if(Details.ContainsKey("@day"))
			{
				AiredDate = DateTime.Parse(Details["@day"]);
			}
			if(Details.ContainsKey("@desc"))
			{
				Overview = Details["desc"];
			}
			if(Details.ContainsKey("tvnn"))
			{
				TVNetwork = Details["tvnn"];
			}

			_HasExtractedDetails = true;
			return true;
		}
	}
}

