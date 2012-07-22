using System;
using System.Linq;
using System.Files;
using System.Reflection;
using System.Collections.Generic;
using TvdbLib;
using TvdbLib.Data;
using TvdbLib.Data.Banner;
using TvdbLib.Cache;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsTVDB : IShowDetailsBasic, IShowDetailsAdditional
	{
		private static Directory CacheDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name.Replace(".", FileSystem.DirectorySeperator.ToString()), "TVDBCache"));
		private static TvdbHandler TVDB = new TvdbHandler(new XmlCacheProvider(CacheDirectory.FullName), "416920BF8A4C278C");

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

		private IEnumerable<IFile> _Artworks;
		public IEnumerable<IFile> Artworks
		{
			get
			{
				return _Artworks;
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
			// Check if cache directory exists.
			if(!CacheDirectory.Exists)
			{
				CacheDirectory.Create();
			}

			// Initalise cache if needed.
			if(!TVDB.IsCacheInitialised)
			{
				TVDB.InitCache();
			}

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
				true
			);

			// Get episode details and banners.
			TvdbEpisode Episode  = null;
			TvdbBanner Banner = null;
			if(ShowDetailsBasic.SeasonNumber == null)
			{
				// Get episode details.
				Episode = Series.GetEpisodesAbsoluteOrder().FindAll(E=>E.IsSpecial==false)[(Int32)ShowDetailsBasic.EpisodeNumber - 1];

				// Poster banner.
				TvdbBanner PosterBanner = Series.PosterBanners.FirstOrDefault();
				if(Banner == null && PosterBanner!=null)
				{
					PosterBanner.LoadBanner();
					Banner = PosterBanner;
				}
			}
			else
			{
				// Get episode details.
				Episode = Series.GetEpisodes(ShowDetailsBasic.SeasonNumber ?? 0).Find(anEpisode => anEpisode.EpisodeNumber == ShowDetailsBasic.EpisodeNumber);
			
				// Season banner;
				TvdbBanner SeasonBanner = Series.SeasonBanners.Where(B=>B.Season==ShowDetailsBasic.SeasonNumber).FirstOrDefault();
				if(SeasonBanner!=null)
				{
					SeasonBanner.LoadBanner();
					Banner = SeasonBanner;
				}
			}

			// Set details.
			_ShowName = Series.SeriesName;
			_SeasonNumber = ShowDetailsBasic.SeasonNumber;
			_EpisodeNumber = ShowDetailsBasic.EpisodeNumber;
			_EpisodeName = Episode.EpisodeName;
			_AiredDate = Episode.FirstAired;
			_Overview = Episode.Overview;
			_TVNetwork = Series.Network;
			_Artworks = (Banner==null)?null:new List<IFile> {GetBannerCacheFile(Banner)};
			_HasDetails = true;
			return true;
		}

		private static IFile GetBannerCacheFile(TvdbBanner Banner, Boolean Thumbnail=false)
		{
			String FileName = "";

			// Add pre-fix based on if thumb or not.
			if(Banner.GetType()==typeof(TvdbBannerWithThumb))
			{
				FileName += "thumb_";
			}
			else
			{
				FileName += "img_";
			}

			// Handle different BannerPath conversion for different banner types.
			if(Banner.GetType() == typeof(TvdbFanartBanner))
			{
				FileName += "fan_" + new File(Banner.BannerPath).Name;
			}
			else
			{
				FileName += Banner.BannerPath.Replace("/", "_");
			}

			// Return file.
			return new File(FileSystem.PathCombine(CacheDirectory.FullName, Banner.SeriesId.ToString(), FileName));
		}
	}
}

