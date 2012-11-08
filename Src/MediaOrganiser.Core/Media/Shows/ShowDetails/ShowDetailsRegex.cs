using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsRegex : IShowDetailsBasic
	{
		private static string ShowNamePatternPreFix = @"\[\w*\][\W_]?";
		private static string ShowNamePatternShowName = @"(?<ShowName>.+)";
		private static string ShowNamePatternSeasonEpisode = @"[Ss](?<SeasonNumber>\d+)[Ee](?<EpisodeNumber>\d+)";
		private static string ShowNamePatternSeasonEpisodeCross = @"(?<SeasonNumber>\d+)x(?<EpisodeNumber>\d{2,})";
		private static string ShowNamePatternEpisodeOnly = @"E(?<EpisodeNumber>\d{2,})";
		private static string ShowNamePatternEpisodeNumberOnly = @"(?<EpisodeNumber>\d{3,})";

		private static IEnumerable<Regex> _Patterns = null;
		private static IEnumerable<Regex> Patterns
		{
			get
			{
				if(_Patterns==null)
				{
					var ShowNamePattern1 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", ShowNamePatternPreFix, ShowNamePatternShowName, ShowNamePatternSeasonEpisode));
					var ShowNamePattern2 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", ShowNamePatternPreFix, ShowNamePatternShowName, ShowNamePatternSeasonEpisodeCross));
					var ShowNamePattern3 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", ShowNamePatternPreFix, ShowNamePatternShowName, ShowNamePatternEpisodeOnly));
					var ShowNamePattern4 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", ShowNamePatternPreFix, ShowNamePatternShowName, ShowNamePatternEpisodeNumberOnly));
					_Patterns = new List<Regex>() { ShowNamePattern1, ShowNamePattern2, ShowNamePattern3, ShowNamePattern4 };
				}
				return _Patterns;
			}
		}

		private String _ShowName;
		public String ShowName
		{
			get
			{
				return _ShowName;
			}
		}

		private Int32? _SeasonNumber;
		public Int32? SeasonNumber
		{
			get
			{
				return _SeasonNumber;
			}
		}

		private Int32? _EpisodeNumber;
		public Int32? EpisodeNumber
		{
			get
			{
				return _EpisodeNumber;
			}
		}

		private Boolean _HasDetails = false;
		public Boolean HasDetails
		{
			get
			{
				return _HasDetails;
			}
		}

		public Boolean ExtractDetails(String SearchInput)
		{
			// Use regex to extract out details.
			foreach(var Pattern in Patterns)
			{
				// Use regex to extract out details.
				var Match = Pattern.Match(SearchInput);
				if(!Match.Success)
				{
					continue;
				}

				// Set the details.
				_ShowName = Regex.Replace(Match.Groups["ShowName"].Value, @"[\W_]+", " ").Trim();
				_ShowName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_ShowName.ToLower());
				
				if(!string.IsNullOrEmpty(Match.Groups["SeasonNumber"].Value))
				{
					_SeasonNumber = Int32.Parse(Match.Groups["SeasonNumber"].Value);
				}
				_EpisodeNumber = Int32.Parse(Match.Groups["EpisodeNumber"].Value);
				
				_HasDetails = true;
				return true;
			}

			return false;
		}
	}
}

