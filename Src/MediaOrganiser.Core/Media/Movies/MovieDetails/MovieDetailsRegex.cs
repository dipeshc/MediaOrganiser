using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Media.Movies.Details
{
	public class MovieDetailsRegex : IMovieDetailsBasic
	{
		private static string _moviePatternPreFix = @"[\[{(].+[\]})][\W_]?";
		private static string _moviePatternName = @"(?<Name>.+)";
		private static string _moviePatternYear = @"(?<Year>\d{4})";

		private static IEnumerable<Regex> _patterns = null;
		private static IEnumerable<Regex> patterns
		{
			get
			{
				if(_patterns==null)
				{
					var moviePattern1 = new Regex(string.Format(@"^(?:{0})?{1}(?=[\W_]{2})", _moviePatternPreFix, _moviePatternName, _moviePatternYear));
					_patterns = new List<Regex>() { moviePattern1 };
				}
				return _patterns;
			}
		}

		public string Name { get; private set; }
		public int Year { get; private set; }
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
				Name = Regex.Replace(Match.Groups["Name"].Value, @"[().+_-]+", " ").Trim();
				Year = Int32.Parse(Match.Groups["Year"].Value);
				
				HasDetails = true;
				return true;
			}

			return false;
		}
	}
}

