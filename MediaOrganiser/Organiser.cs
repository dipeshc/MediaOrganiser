using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading.Tasks;
using System.Threading.Helpers;
using System.Collections.Generic;
using MediaOrganiser.Media;
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

		// ThreadAvailability for each action.
		private LockableInt CopyMediaToWorkingAreaThreadAvailability = new LockableInt(1);
		private LockableInt ConvertMediaThreadAvailability = new LockableInt(1);
		private LockableInt ExtractExhaustiveMediaDetailsThreadAvailability = new LockableInt(1);
		private LockableInt SaveMediaMetaDataThreadAvailability = new LockableInt(2);
		private LockableInt RenameMediaToCleanNameThreadAvailability = new LockableInt(5);
		private LockableInt AddMediaToiTunesThreadAvailability = new LockableInt(1);
		private LockableInt DeleteMediaThreadAvailability = new LockableInt(10);
		private LockableInt MoveMediaToOutputDirectoryThreadAvailability = new LockableInt(5);

		public Organiser (IEnumerable<IPath> InputPaths, IDirectory OutputDirectory, IEnumerable<IPath> ExcludedPaths, Boolean AddToiTunes, Boolean ExcludeiTunesMedia)
		{
			// Setup folders.
			this.InputPaths = InputPaths;
			this.OutputDirectory = OutputDirectory;
			this.WorkingDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), "MediaOrganiser", "WorkingArea"));
			this.AddToiTunes = AddToiTunes;

			this.ExcludedPaths = ExcludedPaths??new List<IPath>();
			if(OutputDirectory!=null)
			{
				Enumerable.Union<IPath>(new List<IPath>{new Path(OutputDirectory.FullName)}, this.ExcludedPaths);
			}
			if(ExcludeiTunesMedia)
			{
				this.ExcludedPaths = Enumerable.Union<IPath>(new List<IPath>{new Path(Apple.iTunes.Properties.RootMediaDirectory.FullName)}, this.ExcludedPaths);
			}

			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}
		}

		public void Organise()
		{
			// Find media that needs to be organised.
			Log.WriteLine("Getting media to be organised");
			IEnumerable<IMedia> MediaToOrganise = ShowFinder.FindShowsToOrganise(InputPaths, ExcludedPaths);
			Log.WriteLine("MediaToOrganise Found. Number: {0}", new List<IMedia>(MediaToOrganise).Count);

			// Organise media.
			Parallel.ForEach(MediaToOrganise, Media =>
			{
				// Copy media to working area.
				Helper.RunWhenThreadAvailable(CopyMediaToWorkingAreaThreadAvailability, 1, () =>
				{
					CopyMediaToWorkingArea(Media);
				});

				if(Media.RequiresConversion)
				{
					Helper.RunWhenThreadAvailable(ConvertMediaThreadAvailability, 1, () =>
					{
						ConvertMedia(Media);
					});
				}

				Helper.RunWhenThreadAvailable(ExtractExhaustiveMediaDetailsThreadAvailability, 1, () =>
				{
					ExtractExhaustiveMediaDetails(Media);
				});

				Helper.RunWhenThreadAvailable(SaveMediaMetaDataThreadAvailability, 1, () =>
				{
					SaveMediaMetaData(Media);
				});

				Helper.RunWhenThreadAvailable(RenameMediaToCleanNameThreadAvailability, 1, () =>
				{
					RenameMediaToCleanFileName(Media);
				});

				if(AddToiTunes)
				{
					Helper.RunWhenThreadAvailable(AddMediaToiTunesThreadAvailability, 1, () =>
					{
						AddMediaToiTunes(Media);
					});
				}

				if(OutputDirectory==null)
				{
					Helper.RunWhenThreadAvailable(DeleteMediaThreadAvailability, 1, () =>
					{
						DeleteMedia(Media);
					});
				}
				else
				{
					Helper.RunWhenThreadAvailable(MoveMediaToOutputDirectoryThreadAvailability, 1, () =>
					{
						MoveMediaToOutputDirectory(Media);
					});
				}
			});
		}

		private void CopyMediaToWorkingArea(IMedia Media)
		{
			Log.WriteLine("Copying media to working area. {0}", Media.MediaFile.FullName);
			// Create file for working area version of media.
			IFile WorkingAreaMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.MediaFile.Name));

			// Copy the media and then assign the new file to the media.
			Media.MediaFile.CopyTo(WorkingAreaMediaFile, true);
			Media.MediaFile = WorkingAreaMediaFile;
			Log.WriteLine("Copied media to working area. {0}", Media.MediaFile.FullName);
		}

		private void ConvertMedia(IMedia Media)
		{
			Log.WriteLine("Starting media conversion. {0}", Media.MediaFile.FullName);
			Media.Convert();
			Log.WriteLine("Converted media. {0}", Media.MediaFile.FullName);
		}

		private void ExtractExhaustiveMediaDetails(IMedia Media)
		{
			Log.WriteLine("Extracting Details Exhaustive. {0}", Media.MediaFile.FullName);
			Media.ExtractDetails(true);
			Log.WriteLine("Extracted Details Exhaustive. {0}", Media.MediaFile.FullName);
		}

		private void SaveMediaMetaData(IMedia Media)
		{
			Log.WriteLine("Saving details. {0}", Media.MediaFile.FullName);
			Media.SaveDetails();
			Log.WriteLine("Saved details. {0}", Media.MediaFile.FullName);
		}

		private void RenameMediaToCleanFileName(IMedia Media)
		{
			Log.WriteLine("Renaming media. {0}", Media.MediaFile.FullName);
			Media.MediaFile.MoveTo(FileSystem.PathCombine(WorkingDirectory.FullName, Media.CleanFileName), true);
			Log.WriteLine("Renamed media. {0}", Media.MediaFile.FullName);
		}

		private void AddMediaToiTunes(IMedia Media)
		{
			Log.WriteLine("Adding media to iTunes. {0}", Media.MediaFile.FullName);
			Apple.iTunes.Importer.Add(Media.MediaFile);
			Log.WriteLine("Added media to iTunes. {0}", Media.MediaFile.FullName);
		}

		private void DeleteMedia(IMedia Media)
		{
			String MediaFileFullName = Media.MediaFile.FullName;
			Log.WriteLine("Deleting media. {0}", MediaFileFullName);
			Media.MediaFile.Delete();
			Log.WriteLine("Deleted media. {0}", MediaFileFullName);
		}

		private void MoveMediaToOutputDirectory(IMedia Media)
		{
			Log.WriteLine("Copying media to output directory. {0}", Media.MediaFile.FullName);
			Media.MediaFile.MoveTo(FileSystem.PathCombine(OutputDirectory.FullName, Media.MediaFile.Name));
			Log.WriteLine("Copied media to output directory. {0}", Media.MediaFile.FullName);
		}
	}
}

