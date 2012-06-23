using System;
using System.Collections.Generic;
using TvdbLib;
using TvdbLib.Data;

namespace MediaOrganiser.Shows.Details
{
	public class ShowDetailsAdditionalTVDB : IShowDetailsAdditional
	{
		private static TvdbHandler TVDB = new TvdbHandler("416920BF8A4C278C");

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

		public Boolean ExtractDetails(IShowDetailsBasic ShowDetailsBasic)
		{
			// Get details from the TVDB.
			List<TvdbSearchResult> SearchResults = TVDB.SearchSeries(ShowDetailsBasic.ShowName);
			
			// If no details found then return false.
			if(SearchResults.Count == 0)
			{
				return false;
			}
			
			// Get details.
			TvdbSeries Series = TVDB.GetSeries(
				SearchResults[0].Id,
				TvdbLanguage.DefaultLanguage,
				true,
				false,
				false
			);
			TvdbEpisode Episode = Series.GetEpisodes(ShowDetailsBasic.SeasonNumber ?? 0).Find(anEpisode => anEpisode.EpisodeNumber == ShowDetailsBasic.EpisodeNumber);
			
			// Set details.
			EpisodeName = Episode.EpisodeName;
			AiredDate = Episode.FirstAired;
			Overview = Episode.Overview;
			TVNetwork = Series.Network;

			_HasExtractedDetails = true;
			return true;
		}
	}
}

