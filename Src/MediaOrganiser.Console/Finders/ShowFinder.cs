using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaOrganiser.Media;
using MediaOrganiser.Media.Shows;

namespace MediaOrganiser.Console.Finders
{
	public class ShowFinder : IMediaFinder
	{
		private static IFileSystem fileSystem = new FileSystem();
		public static IEnumerable<String> ShowFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};

		private List<IShow> InputShows;
		private List<IShow> ExcludedShows;

		public IEnumerable<string> InputPaths { get; private set; }
		public IEnumerable<string> ExcludedPaths { get; private set; }

		public ShowFinder(IEnumerable<string> InputPaths, IEnumerable<string> ExcludedPaths)
		{
			this.InputPaths = InputPaths;
			this.ExcludedPaths = ExcludedPaths;
		}


		public IEnumerable<IMedia> Scan()
		{
			List<IShow> InputShowsTemp = new List<IShow>(GetShowsAtPaths(InputPaths));
			List<IShow> ExcludedShowsTemp = new List<IShow>(GetShowsAtPaths(ExcludedPaths));

			Parallel.ForEach(Enumerable.Union<IMedia>(InputShowsTemp, ExcludedShowsTemp), Show =>
			{
				System.Console.WriteLine("Extracting show details (not full) for {0}", Show.MediaFile.FullName);
				Show.ExtractDetails(false);
				System.Console.WriteLine("Extracted show details (not full) for {0}", Show.MediaFile.FullName);
			});

			InputShows = InputShowsTemp.FindAll(Show => Show.HasDetails);
			ExcludedShows = ExcludedShowsTemp.FindAll(Show => Show.HasDetails);

			return new List<IShow>(InputShows).FindAll(InputShow=>
			{
				return !ExcludedShows.Any(ExcludedShow => AreShowsSame(ExcludedShow, InputShow));
			});
		}


		private IEnumerable<IShow> GetShowsAtPaths(IEnumerable<string> Paths)
		{
			var Shows = new List<IShow>();
			foreach(var Path in Paths)
			{
				Shows.AddRange(GetShowsAtPath(Path));
			}
			return Shows;
		}


		private IEnumerable<IShow> GetShowsAtPath(string Path)
		{
			var Shows = new List<IShow>();
			foreach(var ShowFileType in ShowFileTypes)
			{
				// If file then add directly.
				if(fileSystem.File.Exists(Path) && fileSystem.FileInfo.FromFileName(Path).Extension.ToLower()=="."+ShowFileType)
				{
					Shows.Add(new Show(fileSystem.FileInfo.FromFileName(Path)));
				}
				else if(fileSystem.Directory.Exists(Path))
				{
					// If directory go through directory and then add.
					foreach(var ShowFile in fileSystem.DirectoryInfo.FromDirectoryName(Path).GetFiles("*."+ShowFileType, System.IO.SearchOption.AllDirectories))
					{
						Shows.Add(new Show(ShowFile));
					}
				}
			}
			return Shows;
		}

		private Boolean AreShowsSame(IShow Show1, IShow Show2)
		{
			// Check if show has details extracted.
			if(!Show1.HasDetails)
			{
				throw new Exception(String.Format("Show1 must have details extracted for comparison. {0}", Show1.MediaFile.FullName));
			}
			if(!Show2.HasDetails)
			{
				throw new Exception(String.Format("Show2 must have details extracted for comparison. {0}", Show2.MediaFile.FullName));
			}

			return (Show1.ShowName.ToLower() == Show2.ShowName.ToLower()
			        && Show1.SeasonNumber == Show2.SeasonNumber
			        && Show1.EpisodeNumber == Show2.EpisodeNumber);
		}
	}
}

