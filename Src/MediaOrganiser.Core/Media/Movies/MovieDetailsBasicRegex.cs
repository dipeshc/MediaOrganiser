using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaOrganiser.Media.Movies.Details
{
	public class MovieDetailsBasicRegex
	{
		private static Regex ShowPartsRegex = new Regex(@"^(?:\[\w*\][\W_]?)?(.*)(?=[\W_](?:[sS]?(\d+)[eExX])?(\d{2,}))");

		private String _Name;
		public String Name
		{
			get
			{
				return _Name;
			}
		}

		private DateTime? _Year;
		public DateTime? Year
		{
			get
			{
				return _Year;
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

		public Boolean ExtractDetails(String Name)
		{
			// Use regex to extract out details.
			Match Match = ShowPartsRegex.Match(Name);
			if(!Match.Success)
			{
				return false;
			}
			
			// Set the details.
			_Name = Regex.Replace(Match.Groups[1].Value, @"[\W_]+", " ").Trim();
			_Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(_Name.ToLower());
			
			if(Match.Groups[2].Value != "")
			{
				_Year = DateTime.Parse(Match.Groups[2].Value);
			}

			_HasDetails = true;
			return true;
		}
	}
}

