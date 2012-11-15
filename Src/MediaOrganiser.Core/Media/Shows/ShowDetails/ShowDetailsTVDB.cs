using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using TvdbLib;
using TvdbLib.Data;
using TvdbLib.Data.Banner;
using TvdbLib.Cache;

namespace MediaOrganiser.Media.Shows.Details
{
	public class ShowDetailsTVDB : IShowDetailsBasic, IShowDetailsAdditional
	{
		private static IFileSystem fileSystem = new FileSystem();
		private static DirectoryInfoBase CacheDirectory = fileSystem.DirectoryInfo.FromDirectoryName(fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), "MediaOrganiser" + fileSystem.Path.DirectorySeparatorChar +"TVDBCache"));
		private static TvdbHandler TVDB = new TvdbHandler(new XmlCacheProvider(CacheDirectory.FullName), "416920BF8A4C278C");

		public string ShowName { get; private set; }
		public int? SeasonNumber { get; private set; }
		public int? EpisodeNumber { get; private set; }
		public string EpisodeName { get; private set; }
		public DateTime? AiredDate { get; private set; }
		public string Overview { get; private set; }
		public string TVNetwork { get; private set; }
		public IEnumerable<FileInfoBase> Artworks { get; private set; }
		public bool HasDetails { get; private set; }

		public bool ExtractDetails(IShowDetailsBasic showDetailsBasic)
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
			List<TvdbSearchResult> searchResults = TVDB.SearchSeries(showDetailsBasic.ShowName);
			
			// If no details found then return false.
			if(searchResults.Count == 0)
			{
				return false;
			}
			
			// Get details.
			var series = TVDB.GetSeries(
				searchResults[0].Id,
				TvdbLanguage.DefaultLanguage,
				true,
				false,
				true
			);

			// Get episode details and banners.
			TvdbEpisode episode  = null;
			TvdbBanner banner = null;
			if(showDetailsBasic.SeasonNumber == null)
			{
				// Get episode details.
				episode = series.GetEpisodesAbsoluteOrder().FindAll(E=>E.IsSpecial==false)[(int)showDetailsBasic.EpisodeNumber - 1];

				// Poster banner.
				var posterBanner = series.PosterBanners.FirstOrDefault();
				if(banner == null && posterBanner!=null)
				{
					posterBanner.LoadBanner();
					banner = posterBanner;
				}
			}
			else
			{
				// Get episode details.
				episode = series.GetEpisodes(showDetailsBasic.SeasonNumber ?? 0).Find(anEpisode => anEpisode.EpisodeNumber == showDetailsBasic.EpisodeNumber);
			
				// Season banner;
				var seasonBanner = series.SeasonBanners.Where(B=>B.Season==showDetailsBasic.SeasonNumber).FirstOrDefault();
				if(seasonBanner!=null)
				{
					seasonBanner.LoadBanner();
					banner = seasonBanner;
				}
			}

			// Set details.
			ShowName = series.SeriesName;
			SeasonNumber = showDetailsBasic.SeasonNumber;
			EpisodeNumber = showDetailsBasic.EpisodeNumber;
			EpisodeName = episode.EpisodeName;
			AiredDate = episode.FirstAired;
			Overview = episode.Overview;
			TVNetwork = series.Network;
			Artworks = (banner==null)?null:new List<FileInfoBase> {GetBannerCacheFile(banner)};
			HasDetails = true;
			return true;
		}

		private static FileInfoBase GetBannerCacheFile(TvdbBanner banner, Boolean thumbnail=false)
		{
			// Initalise empty filename..
			String fileName = "";

			// Add pre-fix based on if thumb or not.
			if(banner.GetType()==typeof(TvdbBannerWithThumb))
			{
				fileName += "thumb_";
			}
			else
			{
				fileName += "img_";
			}

			// Handle different BannerPath conversion for different banner types.
			if(banner.GetType() == typeof(TvdbFanartBanner))
			{
				fileName += "fan_" + fileSystem.FileInfo.FromFileName(banner.BannerPath).Name;
			}
			else
			{
				fileName += banner.BannerPath.Replace("/", "_");
			}

			// Return file.
			var innerPath = fileSystem.Path.Combine(banner.SeriesId.ToString(), fileName);
			return fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(CacheDirectory.FullName, innerPath));
		}
	}
}

