using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsRegex : IShowDetailsBasic
	{
		private static string _showNamePatternPreFix = @"\[\w*\][\W_]?";
		private static string _showNamePatternShowName = @"(?<ShowName>.+)";
		private static string _showNamePatternSeasonEpisode = @"[Ss](?<SeasonNumber>\d+)[Ee](?<EpisodeNumber>\d+)";
		private static string _showNamePatternSeasonEpisodeCross = @"(?<SeasonNumber>\d+)x(?<EpisodeNumber>\d{2,})";
		private static string _showNamePatternEpisodeOnly = @"E(?<EpisodeNumber>\d{2,})";
		private static string _showNamePatternEpisodeNumberOnly = @"(?<EpisodeNumber>\d{3,})";

		private static IEnumerable<Regex> _patterns = null;
		private static IEnumerable<Regex> patterns
		{
			get
			{
				if(_patterns==null)
				{
					var ShowNamePattern1 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", _showNamePatternPreFix, _showNamePatternShowName, _showNamePatternSeasonEpisode));
					var ShowNamePattern2 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", _showNamePatternPreFix, _showNamePatternShowName, _showNamePatternSeasonEpisodeCross));
					var ShowNamePattern3 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", _showNamePatternPreFix, _showNamePatternShowName, _showNamePatternEpisodeOnly));
					var ShowNamePattern4 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", _showNamePatternPreFix, _showNamePatternShowName, _showNamePatternEpisodeNumberOnly));
					_patterns = new List<Regex>() { ShowNamePattern1, ShowNamePattern2, ShowNamePattern3, ShowNamePattern4 };
				}
				return _patterns;
			}
		}

		public string ShowName { get; private set; }
		public int? SeasonNumber { get; private set; }
		public int? EpisodeNumber { get; private set; }
		public bool HasDetails { get; private set; }

		public bool ExtractDetails(String SearchInput)
		{
			// Use regex to extract out details.
			foreach(var Pattern in patterns)
			{
				// Use regex to extract out details.
				var Match = Pattern.Match(SearchInput);
				if(!Match.Success)
				{
					continue;
				}

				// Set the details.
				ShowName = Regex.Replace(Match.Groups["ShowName"].Value, @"[\W_]+", " ").Trim();
				ShowName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ShowName.ToLower());
				
				if(!string.IsNullOrEmpty(Match.Groups["SeasonNumber"].Value))
				{
					SeasonNumber = Int32.Parse(Match.Groups["SeasonNumber"].Value);
				}
				EpisodeNumber = Int32.Parse(Match.Groups["EpisodeNumber"].Value);
				
				HasDetails = true;
				return true;
			}

			return false;
		}
	}
}

