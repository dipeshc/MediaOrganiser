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
		public static IEnumerable<String> ShowFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};

		private IFileSystem _fileSystem;
		private List<IShow> _inputShows;
		private List<IShow> _excludedShows;

		public IEnumerable<string> InputPaths { get; private set; }
		public IEnumerable<string> ExcludedPaths { get; private set; }

		public ShowFinder(IEnumerable<string> inputPaths, IEnumerable<string> excludedPaths) : this(new FileSystem(), inputPaths, excludedPaths)
		{
		}

		public ShowFinder(IFileSystem fileSystem, IEnumerable<string> inputPaths, IEnumerable<string> excludedPaths)
		{
			_fileSystem = fileSystem;
			InputPaths = inputPaths;
			ExcludedPaths = excludedPaths;
		}

		public IEnumerable<IMedia> Scan()
		{
			var inputShowsTemp = new List<IShow>(InputPaths.SelectMany(path => GetShowsAtPath(path)));
			var excludedShowsTemp = new List<IShow>(ExcludedPaths.SelectMany(path => GetShowsAtPath(path)));

			Parallel.ForEach(Enumerable.Union<IMedia>(inputShowsTemp, excludedShowsTemp), show =>
			{
				System.Console.WriteLine("Extracting show details (not full) for {0}", show.MediaFile.FullName);
				show.ExtractDetails(false);
				System.Console.WriteLine("Extracted show details (not full) for {0}", show.MediaFile.FullName);
			});

			_inputShows = inputShowsTemp.FindAll(show => show.HasDetails);
			_excludedShows = excludedShowsTemp.FindAll(show => show.HasDetails);

			return new List<IShow>(_inputShows).FindAll(inputShow=>
			{
				return !_excludedShows.Any(excludedShow => AreShowsSame(excludedShow, inputShow));
			});
		}

		private IEnumerable<IShow> GetShowsAtPath(string Path)
		{
			var shows = new List<IShow>();
			foreach(var showFileType in ShowFileTypes)
			{
				// If file then add directly.
				if(_fileSystem.File.Exists(Path) && _fileSystem.FileInfo.FromFileName(Path).Extension.ToLower()=="."+showFileType)
				{
					shows.Add(new Show(_fileSystem, _fileSystem.FileInfo.FromFileName(Path)));
				}
				else if(_fileSystem.Directory.Exists(Path))
				{
					// If directory go through directory and then add.
					foreach(var ShowFile in _fileSystem.DirectoryInfo.FromDirectoryName(Path).GetFiles("*."+showFileType, System.IO.SearchOption.AllDirectories))
					{
						shows.Add(new Show(_fileSystem, ShowFile));
					}
				}
			}
			return shows;
		}

		private Boolean AreShowsSame(IShow show1, IShow show2)
		{
			// Check if show has details extracted.
			if(!show1.HasDetails)
			{
				throw new Exception(String.Format("Show1 must have details extracted for comparison. {0}", show1.MediaFile.FullName));
			}
			if(!show2.HasDetails)
			{
				throw new Exception(String.Format("Show2 must have details extracted for comparison. {0}", show2.MediaFile.FullName));
			}

			return (show1.ShowName.ToLower() == show2.ShowName.ToLower()
			        && show1.SeasonNumber == show2.SeasonNumber
			        && show1.EpisodeNumber == show2.EpisodeNumber);
		}
	}
}

