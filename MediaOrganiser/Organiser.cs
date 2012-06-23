using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading.Tasks;
using System.Threading.Helpers;
using System.Collections.Generic;
using MediaOrganiser.Shows;
using MediaOrganiser.Convertor;

namespace MediaOrganiser
{
	public class Organiser
	{
		private IEnumerable<IDirectory> InputDirectories;
		private IDirectory OutputDirectory;
		private IEnumerable<IDirectory> ExcludedDirectories;
		private IDirectory WorkingDirectory;

		public IEnumerable<String> ShowInputFileTypes = new List<String>() {"mp4", "avi", "mkv", "m4v"};
		public String OutputFileType = "mp4";

		// ThreadAvailability for each action.
		private LockableInt CopyShowToWorkingAreaThreadAvailability = new LockableInt(1);
		private LockableInt ConvertShowIfRequiredThreadAvailability = new LockableInt(1);
		private LockableInt ExtractAdditionalShowDetailsIfPossibleThreadAvailability = new LockableInt(1);
		private LockableInt SaveShowMetaDataThreadAvailability = new LockableInt(2);
		private LockableInt MoveShowToOutputDirectoryThreadAvailability = new LockableInt(5);

		public Organiser (IEnumerable<IDirectory> InputDirectories, IDirectory OutputDirectory, IEnumerable<IDirectory> ExcludedDirectories)
		{
			// Setup folders.
			this.InputDirectories = InputDirectories;
			this.OutputDirectory = OutputDirectory;
			this.ExcludedDirectories = Enumerable.Union<IDirectory>(new List<IDirectory>{OutputDirectory}, ExcludedDirectories);
			this.WorkingDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), "MediaOrganiserWorkingArea"));

			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}
		}

		public void Organise()
		{
			// Scan input and output folder to identify files.
			IEnumerable<IShow> InputShows = ShowFinder.GetShowsInDirectories(InputDirectories, ShowInputFileTypes);
			IEnumerable<IShow> ExcludedShows = ShowFinder.GetShowsInDirectories(ExcludedDirectories, ShowInputFileTypes);

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

				Helper.RunWhenThreadAvailable(MoveShowToOutputDirectoryThreadAvailability, 1, () =>
				{
					MoveShowToOutputDirectory(Show);
				});
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

		private void MoveShowToOutputDirectory(IShow Show)
		{
			Log.WriteLine("Copying show to output directory. {0}", Show.ShowFile.FullName);
			Show.ShowFile.MoveTo(FileSystem.PathCombine(OutputDirectory.FullName, Show.ShowFileName));
			Log.WriteLine("Copied show to output directory. {0}", Show.ShowFile.FullName);
		}
	}
}

