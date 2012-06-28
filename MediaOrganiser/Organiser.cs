using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading.Tasks;
using System.Threading.Helpers;
using System.Collections.Generic;
using MediaOrganiser.Shows;
using MediaOrganiser.Convertor;
using Apple.iTunes;

namespace MediaOrganiser
{
	public class Organiser
	{
		private IEnumerable<IPath> InputPaths;
		private IDirectory OutputDirectory;
		private IEnumerable<IPath> ExcludedPaths;
		private IDirectory WorkingDirectory;
		private Boolean AddToiTunes;

		public IEnumerable<String> ShowInputFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};
		public String OutputFileType = "mp4";

		// ThreadAvailability for each action.
		private LockableInt CopyShowToWorkingAreaThreadAvailability = new LockableInt(1);
		private LockableInt ConvertShowIfRequiredThreadAvailability = new LockableInt(1);
		private LockableInt ExtractAdditionalShowDetailsIfPossibleThreadAvailability = new LockableInt(1);
		private LockableInt SaveShowMetaDataThreadAvailability = new LockableInt(2);
		private LockableInt AddShowToiTunesThreadAvailability = new LockableInt(1);
		private LockableInt DeleteShowThreadAvailability = new LockableInt(10);
		private LockableInt MoveShowToOutputDirectoryThreadAvailability = new LockableInt(5);

		public Organiser (IEnumerable<IPath> InputPaths, IDirectory OutputDirectory, IEnumerable<IPath> ExcludedPaths, Boolean AddToiTunes, Boolean ExcludeiTunesMedia)
		{
			// Setup folders.
			this.InputPaths = InputPaths;
			this.OutputDirectory = OutputDirectory;
			this.WorkingDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), "MediaOrganiserWorkingArea"));
			this.AddToiTunes = AddToiTunes;

			this.ExcludedPaths = ExcludedPaths??new List<IPath>();
			if(OutputDirectory!=null)
			{
				Enumerable.Union<IPath>(new List<IPath>{new Path(OutputDirectory.FullName)}, this.ExcludedPaths);
			}
			if(ExcludeiTunesMedia)
			{
				this.ExcludedPaths = Enumerable.Union<IPath>(new List<IPath>{new Path(Apple.iTunes.Properties.TVShowMediaDirectory.FullName)}, this.ExcludedPaths);
			}

			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}
		}

		public void Organise()
		{
			// Scan input and output folder to identify files.
			IEnumerable<IShow> InputShows = ShowFinder.GetShows(InputPaths, ShowInputFileTypes);
			IEnumerable<IShow> ExcludedShows = ShowFinder.GetShows(ExcludedPaths, ShowInputFileTypes);

			// Extract basic details from show.
			Parallel.ForEach(Enumerable.Union<IShow>(InputShows, ExcludedShows), Show =>
			{
				Log.WriteLine("Extracting Basic Details. {0}", Show.ShowFile.FullName);
				Show.ExtractBasicDetails();
				Log.WriteLine("Extracted Basic Details. {0}", Show.ShowFile.FullName);
			});

			// Identify files that are currently not in the output directory.
			Log.WriteLine("Getting NonExcludedShows");
			IEnumerable<IShow> ShowsToOrganise = ShowFinder.GetNonExcludedShows(InputShows, ExcludedShows);
			Log.WriteLine("NonExcludedShows Found. Number: {0}", new List<IShow>(ShowsToOrganise).Count);

			// Organise shows.
			Parallel.ForEach(ShowsToOrganise, Show =>
			{
				Helper.RunWhenThreadAvailable(CopyShowToWorkingAreaThreadAvailability, 1, () =>
				{
					CopyShowToWorkingArea(Show);
				});

				Helper.RunWhenThreadAvailable(ConvertShowIfRequiredThreadAvailability, 1, () =>
				{
					ConvertShowIfRequired(Show);
				});

				Helper.RunWhenThreadAvailable(ExtractAdditionalShowDetailsIfPossibleThreadAvailability, 1, () =>
				{
					ExtractAdditionalShowDetailsIfPossible(Show);
				});

				Helper.RunWhenThreadAvailable(SaveShowMetaDataThreadAvailability, 1, () =>
				{
					SaveShowMetaData(Show);
				});

				if(AddToiTunes)
				{
					Helper.RunWhenThreadAvailable(AddShowToiTunesThreadAvailability, 1, () =>
					{
						AddShowToiTunes(Show);
					});
				}

				if(OutputDirectory==null)
				{
					Helper.RunWhenThreadAvailable(DeleteShowThreadAvailability, 1, () =>
					{
						DeleteShow(Show);
					});
				}
				else
				{
					Helper.RunWhenThreadAvailable(MoveShowToOutputDirectoryThreadAvailability, 1, () =>
					{
						MoveShowToOutputDirectory(Show);
					});
				}
			});
		}

		private void CopyShowToWorkingArea(IShow Show)
		{
			Log.WriteLine("Copying show to working area. {0}", Show.ShowFile.FullName);
			// Create file for working area version of show.
			IFile WorkingAreaShowFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Show.ShowFile.Name));

			// Copy the show and then assign the new file to the show.
			Show.ShowFile.CopyTo(WorkingAreaShowFile, true);
			Show.ShowFile = WorkingAreaShowFile;
			Log.WriteLine("Copied show to working area. {0}", Show.ShowFile.FullName);
		}

		private void ConvertShowIfRequired(IShow Show)
		{
			Log.WriteLine("Checking if show needs to be converted. {0}", Show.ShowFile.FullName);
			// Check if extension matches output extension. If not then convert.
			if(Show.ShowFile.Extension.ToLower() == "."+OutputFileType)
			{
				Log.WriteLine("Show does not need to be converted. {0}", Show.ShowFile.FullName);
				return;
			}

			// Create file for converted version of show.
			IFile ConvertedShowFile = new File(Show.ShowFile.FullNameWithoutExtension + "." + OutputFileType);

			// Convert show.
			Log.WriteLine("Starting show conversion. {0}", Show.ShowFile.FullName);
			Convertor.Convertor.ConvertForiPad(Show.ShowFile, ConvertedShowFile);

			// Delete old file and assign the new file to the show.
			IFile OldFile = Show.ShowFile;
			Show.ShowFile = ConvertedShowFile;
			OldFile.Delete();
			Log.WriteLine("Converted show if required. {0}", Show.ShowFile.FullName);
		}

		private void ExtractAdditionalShowDetailsIfPossible(IShow Show)
		{
			Log.WriteLine("Extracting additional details. {0}", Show.ShowFile.FullName);
			try
			{
				Show.ExtractAdditionalDetails();
				Log.WriteLine("Extracted additional details. {0}", Show.ShowFile.FullName);
			}
			catch
			{
				Log.WriteLine("Failed to extract additional details. {0}", Show.ShowFile.FullName);
			}
		}

		private void SaveShowMetaData(IShow Show)
		{
			Log.WriteLine("Saving details. {0}", Show.ShowFile.FullName);
			Show.SaveDetails();
			Log.WriteLine("Saved details. {0}", Show.ShowFile.FullName);
		}

		private void AddShowToiTunes(IShow Show)
		{
			Log.WriteLine("Adding show to iTunes. {0}", Show.ShowFile.FullName);
			Apple.iTunes.Importer.Add(Show.ShowFile);
			Log.WriteLine("Added show to iTunes. {0}", Show.ShowFile.FullName);
		}

		private void DeleteShow(IShow Show)
		{
			String ShowFileFullName = Show.ShowFile.FullName;
			Log.WriteLine("Deleting show as no output directory. {0}", ShowFileFullName);
			Show.ShowFile.Delete();
			Log.WriteLine("Deleted show as no output directory. {0}", ShowFileFullName);
		}

		private void MoveShowToOutputDirectory(IShow Show)
		{
			Log.WriteLine("Copying show to output directory. {0}", Show.ShowFile.FullName);
			Show.ShowFile.MoveTo(FileSystem.PathCombine(OutputDirectory.FullName, Show.ShowFileName));
			Log.WriteLine("Copied show to output directory. {0}", Show.ShowFile.FullName);
		}
	}
}

