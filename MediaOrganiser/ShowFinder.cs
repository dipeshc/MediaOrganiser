using System;
using System.Files;
using System.Logging;
using System.Collections.Generic;
using MediaOrganiser.Shows;

namespace MediaOrganiser
{
	public class ShowFinder
	{
		public static IEnumerable<IShow> GetShows(IPath Path, String ShowFileType)
		{
			return GetShows(new List<IPath>{Path}, new List<String>{ShowFileType});
		}

		public static IEnumerable<IShow> GetShows(IPath Path, IEnumerable<String> ShowFileTypes)
		{
			return GetShows(new List<IPath>{Path}, ShowFileTypes);
		}

		public static IEnumerable<IShow> GetShows(IEnumerable<IPath> Paths, String ShowFileType)
		{
			return GetShows(Paths, new List<String>{ShowFileType});
		}

		public static IEnumerable<IShow> GetShows(IEnumerable<IPath> Paths, IEnumerable<String> ShowFileTypes)
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

