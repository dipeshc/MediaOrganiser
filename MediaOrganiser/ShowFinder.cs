using System;
using System.Files;
using System.Logging;
using System.Collections.Generic;
using MediaOrganiser.Shows;

namespace MediaOrganiser
{
	public class ShowFinder
	{
		public static IEnumerable<IShow> GetShowsInDirectory(IDirectory Directory, String ShowFileType)
		{
			return GetShowsInDirectories(new List<IDirectory>{Directory}, new List<String>{ShowFileType});
		}

		public static IEnumerable<IShow> GetShowsInDirectory(IDirectory Directory, IEnumerable<String> ShowFileTypes)
		{
			return GetShowsInDirectories(new List<IDirectory>{Directory}, ShowFileTypes);
		}

		public static IEnumerable<IShow> GetShowsInDirectories(IEnumerable<IDirectory> Directories, String ShowFileType)
		{
			return GetShowsInDirectories(Directories, new List<String>{ShowFileType});
		}

		public static IEnumerable<IShow> GetShowsInDirectories(IEnumerable<IDirectory> Directories, IEnumerable<String> ShowFileTypes)
		{
			IList<IShow> Shows = new List<IShow>();
			foreach(String ShowFileType in ShowFileTypes)
			{
				foreach(IDirectory Directory in Directories)
				{
					foreach(IFile ShowFile in Directory.GetFilesInAllDirectories("*."+ShowFileType))
					{
						Shows.Add(new Show(ShowFile));
					}
				}
			}
			return Shows;
		}

		public static IEnumerable<IShow> GetNonExcludedShows(IEnumerable<IShow> InputShows, IEnumerable<IShow> ExcludedShows)
		{
			IList<IShow> NonExcludedShows = new List<IShow>();

			foreach(IShow InputShow in InputShows)
			{
				// Check if show has details extracted.
				if(!InputShow.HasBasicDetails)
				{
					Log.WriteLine("Basic Details not found. Will not be included for organisation. Skipping show. {0}", InputShow.ShowFile.FullName);
					continue;
				}

				// Search excluded folders for the InputShow.
				Boolean Found = false;
				foreach(IShow ExlcudedShow in ExcludedShows)
				{
					if(InputShow.ShowDetailsBasic.ShowName == ExlcudedShow.ShowDetailsBasic.ShowName &&
					   InputShow.ShowDetailsBasic.SeasonNumber == ExlcudedShow.ShowDetailsBasic.SeasonNumber &&
					   InputShow.ShowDetailsBasic.EpisodeNumber == ExlcudedShow.ShowDetailsBasic.EpisodeNumber)
					{
						Found = true;
						break;
					}
				}

				// If show not found add to NonExcludedShows.
				if(!Found)
				{
					NonExcludedShows.Add(InputShow);
				}
			}

			return NonExcludedShows;
		}
	}
}

