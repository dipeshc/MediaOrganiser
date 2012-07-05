using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaOrganiser.Media;
using MediaOrganiser.Media.Shows;

namespace MediaOrganiser
{
	public class ShowFinder
	{
		public static IEnumerable<String> ShowFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};

		public static IEnumerable<IMedia> FindShowsToOrganise(IEnumerable<IPath> InputPaths, IEnumerable<IPath> ExcludedPaths)
		{
			// Scan input and output folder to identify shows.
			IEnumerable<IShow> InputShows = ShowFinder.GetShows(InputPaths);
			IEnumerable<IShow> ExcludedShows = ShowFinder.GetShows(ExcludedPaths);

			// Extract basic details from show.
			Parallel.ForEach(Enumerable.Union<IShow>(InputShows, ExcludedShows), Show =>
			{
				Log.WriteLine("Extracting non-full detail for show {0}", Show.MediaFile.FullName);
				Show.ExtractDetails(false);
				Log.WriteLine("Extracted non-full detail for show {0}", Show.MediaFile.FullName);
			});

			// Identify shows that needs to be organised.
			return ShowFinder.GetNonExcludedShows(InputShows, ExcludedShows);
		}

		private static IEnumerable<IShow> GetShows(IEnumerable<IPath> Paths)
		{
			IList<IShow> Shows = new List<IShow>();
			foreach(String ShowFileType in ShowFileTypes)
			{
				foreach(IPath Path in Paths)
				{
					// If file then add directly.
					if(Path.IsFile && new File(Path).Extension.ToLower()=="."+ShowFileType)
					{
						Shows.Add(new Show(new File(Path)));
					}
					else if(Path.IsDirectory)
					{
						// If directory go through directory and then add.
						foreach(IFile ShowFile in new Directory(Path).GetFilesInAllDirectories("*."+ShowFileType))
						{
							Shows.Add(new Show(ShowFile));
						}
					}
				}
			}
			return Shows;
		}

		private static IEnumerable<IShow> GetNonExcludedShows(IEnumerable<IShow> InputShows, IEnumerable<IShow> ExcludedShows)
		{
			IList<IShow> NonExcludedShows = new List<IShow>();

			foreach(IShow InputShow in InputShows)
			{
				// Check if show has details extracted.
				if(!InputShow.HasDetails)
				{
					Log.WriteLine("Details not found. Will not be included for organisation. Skipping. {0}", InputShow.MediaFile.FullName);
					continue;
				}

				// Search excluded folders for the InputShow.
				Boolean Found = false;
				foreach(IShow ExlcudedShow in ExcludedShows)
				{
					if(InputShow.ShowName == ExlcudedShow.ShowName &&
					   InputShow.SeasonNumber == ExlcudedShow.SeasonNumber &&
					   InputShow.EpisodeNumber == ExlcudedShow.EpisodeNumber)
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

