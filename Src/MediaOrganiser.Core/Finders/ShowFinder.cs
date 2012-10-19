using System;
using System.Linq;
using System.Files;
using System.Files.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediaOrganiser.Media;
using MediaOrganiser.Media.Shows;

namespace MediaOrganiser.Finders
{
	public class ShowFinder : IMediaFinder
	{
		public System.IO.TextWriter OutputStream = Console.Out;

		private static IEnumerable<String> _FileExtensions = new List<String>() {"mp4", "avi", "mkv", "m4v"};
		public IEnumerable<String> FileExtensions { get { return _FileExtensions; } }

		private List<IShow> InputShows;
		private List<IShow> ExcludedShows;

		private IEnumerable<IPath> _InputPaths;
		public IEnumerable<IPath> InputPaths
		{
			get
			{
				return _InputPaths;
			}
		}
		private IEnumerable<IPath> _ExcludedPaths;
		public IEnumerable<IPath> ExcludedPaths
		{
			get
			{
				return _ExcludedPaths;
			}
		}

		public ShowFinder(IEnumerable<IPath> InputPaths, IEnumerable<IPath> ExcludedPaths)
		{
			this._InputPaths = InputPaths;
			this._ExcludedPaths = ExcludedPaths;
		}


		public IEnumerable<IMedia> Scan()
		{
			List<IShow> InputShowsTemp = new List<IShow>(GetShowsAtPaths(InputPaths));
			List<IShow> ExcludedShowsTemp = new List<IShow>(GetShowsAtPaths(ExcludedPaths));

			Parallel.ForEach(Enumerable.Union<IMedia>(InputShowsTemp, ExcludedShowsTemp), Show =>
			{
				OutputStream.WriteLine("Extracting show details (not full) for {0}", Show.MediaFile.FullName);
				Show.ExtractDetails(false);
				OutputStream.WriteLine("Extracted show details (not full) for {0}", Show.MediaFile.FullName);
			});

			this.InputShows = InputShowsTemp.FindAll(Show => Show.HasDetails);
			this.ExcludedShows = ExcludedShowsTemp.FindAll(Show => Show.HasDetails);

			return new List<IShow>(InputShows).FindAll(InputShow=>
			{
				return !ContainsShow(ExcludedShows, InputShow);
			});
		}

		public void ScanAndWatch(MediaFoundEventHandler Handler)
		{
			// Setup Excludes Watch.
			foreach(IPath Path in new List<IPath>(ExcludedPaths).FindAll(P=>P.IsDirectory))
			{
				System.IO.FileSystemWatcher Watcher = new System.IO.FileSystemWatcher(Path.FullName);
				Watcher.IncludeSubdirectories = true;
				Watcher.Created += (S, E) =>
				{
					new List<IShow>(GetShowsAtPath(new Path(E.FullPath))).ForEach(Show =>
					{
						OutputStream.WriteLine("Extracting show details (not full) for {0}", Show.MediaFile.FullName);
						Show.ExtractDetails(false);
						OutputStream.WriteLine("Extracted show details (not full) for {0}", Show.MediaFile.FullName);
						ExcludedShows.Add(Show);
					});
				};
				Watcher.EnableRaisingEvents = true;
			}

			// Setup Input Watch.
			foreach(IPath Path in new List<IPath>(InputPaths).FindAll(P=>P.IsDirectory))
			{
				System.IO.FileSystemWatcher Watcher = new System.IO.FileSystemWatcher(Path.FullName);
				Watcher.IncludeSubdirectories = true;
				Watcher.Created += (S, E) =>
				{
					new List<IShow>(GetShowsAtPath(new Path(E.FullPath))).FindAll(Show =>
					{
						OutputStream.WriteLine("Extracting show details (not full) for {0}", Show.MediaFile.FullName);
						Show.ExtractDetails(false);
						OutputStream.WriteLine("Extracted show details (not full) for {0}", Show.MediaFile.FullName);
						return !ContainsShow(ExcludedShows, Show);
					}).ForEach(Show =>
					{
						Handler.Invoke(this, Show);
					});
				};
				Watcher.EnableRaisingEvents = true;
			}

			// Run handler on inital scan.
			new List<IMedia>(Scan()).ForEach(Show =>
			{
				Handler.Invoke(this, Show);
			});
		}

		private IEnumerable<IShow> GetShowsAtPaths(IEnumerable<IPath> Paths)
		{
			List<IShow> Shows = new List<IShow>();
			foreach(IPath Path in Paths)
			{
				Shows.AddRange(GetShowsAtPath(Path));
			}
			return Shows;
		}


		private IEnumerable<IShow> GetShowsAtPath(IPath Path)
		{
			List<IShow> Shows = new List<IShow>();
			foreach(String FileExtension in FileExtensions)
			{
				// If file then add directly.
				if(Path.IsFile && new File(Path).Extension.ToLower()=="."+FileExtension)
				{
					Shows.Add(new Show(new File(Path)));
				}
				else if(Path.IsDirectory)
				{
					// If directory go through directory and then add.
					foreach(IFile ShowFile in new Directory(Path).GetFilesInAllDirectories("*."+FileExtension))
					{
						Shows.Add(new Show(ShowFile));
					}
				}
			}
			return Shows;
		}

		private Boolean ContainsShow(IEnumerable<IShow> ListOfShows, IShow Show)
		{
			foreach(IShow ListedShow in ListOfShows)
			{
				if(AreShowsSame(ListedShow, Show))
				{
					return true;
				}
			}

			return false;
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

			return (Show1.ShowName.ToLower() == Show2.ShowName.ToLower() && Show1.SeasonNumber == Show2.SeasonNumber && Show1.EpisodeNumber == Show2.EpisodeNumber);
		}
	}
}

