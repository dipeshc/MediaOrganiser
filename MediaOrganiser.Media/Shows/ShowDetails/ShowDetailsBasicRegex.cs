using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsBasicRegex : IShowDetailsBasic
	{
		private static Regex ShowPartsRegex = new Regex(@"^(?:\[\w*\][\W_]?)?(.*)(?=[\W_](?:[sS]?(\d+)[eExX])?(\d{2,}))");

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
			Match Match = ShowPartsRegex.Match(SearchInput);
			if(!Match.Success)
			{
				return false;
			}
			
			// Set the details.
			_ShowName = Regex.Replace(Match.Groups[1].Value, @"[\W_]+", " ").Trim();
			_ShowName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_ShowName.ToLower());
			
			if(Match.Groups[2].Value != "")
			{
				_SeasonNumber = Int32.Parse(Match.Groups[2].Value);
			}
			_EpisodeNumber = Int32.Parse(Match.Groups[3].Value);

			_HasDetails = true;
			return true;
		}
	}
}

