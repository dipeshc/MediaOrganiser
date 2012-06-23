using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Shows.Details
{
	public class ShowDetailsBasicRegex : IShowDetailsBasic
	{
		private static Regex ShowPartsRegex = new Regex(@"^(?:\[\w*\][\W_]?)?(.*)(?=[\W_](?:[sS]?(\d+)[eExX])?(\d{2,}))");

		public String ShowName { get; set; }
		public Int32? SeasonNumber { get; set; }
		public Int32? EpisodeNumber { get; set; }

		private Boolean _HasExtractedDetails = false;
		public Boolean HasExtractedDetails
		{
			get
			{
				return _HasExtractedDetails;
			}
		}

		public Boolean ExtractDetails(String Name)
		{
			// Use regex to extract out details.
			Match Match = ShowPartsRegex.Match(Name);
			if(!Match.Success)
			{
				return false;
			}
			
			// Set the details.
			ShowName = Regex.Replace(Match.Groups[1].Value, @"[\W_]+", " ").Trim();
			ShowName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ShowName.ToLower());
			
			if(Match.Groups[2].Value != "")
			{
				SeasonNumber = Int32.Parse(Match.Groups[2].Value);
			}
			EpisodeNumber = Int32.Parse(Match.Groups[3].Value);

			_HasExtractedDetails = true;
			return true;
		}
	}
}

