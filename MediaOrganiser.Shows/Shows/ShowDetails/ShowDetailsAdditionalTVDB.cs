using System;
using System.Collections.Generic;
using TvdbLib;
using TvdbLib.Data;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsAdditionalTVDB : IShowDetailsAdditional
	{
		private static TvdbHandler TVDB = new TvdbHandler("416920BF8A4C278C");

		private String _EpisodeName;
		public String EpisodeName
		{
			get
			{
				return _EpisodeName;
			}
		}

		private DateTime? _AiredDate;
		public DateTime? AiredDate
		{
			get
			{
				return _AiredDate;
			}
		}

		private String _Overview;
		public String Overview
		{
			get
			{
				return _Overview;
			}
		}

		private String _TVNetwork;
		public String TVNetwork
		{
			get
			{
				return _TVNetwork;
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
			_EpisodeName = Episode.EpisodeName;
			_AiredDate = Episode.FirstAired;
			_Overview = Episode.Overview;
			_TVNetwork = Series.Network;

			_HasDetails = true;
			return true;
		}
	}
}

