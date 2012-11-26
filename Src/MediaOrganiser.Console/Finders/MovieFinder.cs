using System;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaOrganiser.Media;
using MediaOrganiser.Media.Movies;

namespace MediaOrganiser.Console.Finders
{
	public class MovieFinder : IMediaFinder
	{
		public static IEnumerable<String> MovieFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};

		private IFileSystem _fileSystem;
		private List<IMovie> _inputMovies;
		private List<IMovie> _excludedMovies;

		public IEnumerable<string> InputPaths { get; private set; }
		public IEnumerable<string> ExcludedPaths { get; private set; }

		public MovieFinder(IEnumerable<string> inputPaths, IEnumerable<string> excludedPaths) : this(new FileSystem(), inputPaths, excludedPaths)
		{
		}

		public MovieFinder(IFileSystem fileSystem, IEnumerable<string> inputPaths, IEnumerable<string> excludedPaths)
		{
			_fileSystem = fileSystem;
			InputPaths = inputPaths;
			ExcludedPaths = excludedPaths;
		}

		public IEnumerable<IMedia> Scan()
		{
			var inputShowsTemp = new List<IMovie>(InputPaths.SelectMany(path => GetMoviesAtPath(path)));
			var excludedShowsTemp = new List<IMovie>(ExcludedPaths.SelectMany(path => GetMoviesAtPath(path)));

			Parallel.ForEach(Enumerable.Union<IMedia>(inputShowsTemp, excludedShowsTemp), movie =>
			{
				System.Console.WriteLine("Extracting movie details (not full) for {0}", movie.MediaFile.FullName);
				movie.ExtractDetails(false);
				System.Console.WriteLine("Extracted movie details (not full) for {0}", movie.MediaFile.FullName);
			});

			_inputMovies = inputShowsTemp.FindAll(movie => movie.HasDetails);
			_excludedMovies = excludedShowsTemp.FindAll(movie => movie.HasDetails);

			return new List<IMovie>(_inputMovies).FindAll(inputMovie=>
			{
				return !_excludedMovies.Any(excludedMovie => AreShowsSame(excludedMovie, inputMovie));
			});
		}

		private IEnumerable<IMovie> GetMoviesAtPath(string Path)
		{
			var movies = new List<IMovie>();
			foreach(var movieFileType in MovieFileTypes)
			{
				// If file then add directly.
				if(_fileSystem.File.Exists(Path) && _fileSystem.FileInfo.FromFileName(Path).Extension.ToLower()=="."+movieFileType)
				{
					movies.Add(new Movie(_fileSystem, _fileSystem.FileInfo.FromFileName(Path)));
				}
				else if(_fileSystem.Directory.Exists(Path))
				{
					// If directory go through directory and then add.
					foreach(var ShowFile in _fileSystem.DirectoryInfo.FromDirectoryName(Path).GetFiles("*."+movieFileType, System.IO.SearchOption.AllDirectories))
					{
						movies.Add(new Movie(_fileSystem, ShowFile));
					}
				}
			}
			return movies;
		}

		private Boolean AreShowsSame(IMovie movie1, IMovie movie2)
		{
			// Check if show has details extracted.
			if(!movie1.HasDetails)
			{
				throw new Exception(String.Format("Show1 must have details extracted for comparison. {0}", movie1.MediaFile.FullName));
			}
			if(!movie2.HasDetails)
			{
				throw new Exception(String.Format("Show2 must have details extracted for comparison. {0}", movie2.MediaFile.FullName));
			}

			return string.Compare(movie1.Name, movie2.Name, StringComparison.InvariantCultureIgnoreCase)==0 && (movie1.Year == movie2.Year);
		}
	}
}

