using System;
using System.Linq;
using System.Files;
using System.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Helpers;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using MediaOrganiser.Media;
using Apple.iTunes;

namespace MediaOrganiser.Organisers
{
	public class Organiser : IOrganiser
	{
		private IDirectory _OutputDirectory;
		public IDirectory OutputDirectory
		{
			get
			{
				return _OutputDirectory;
			}
		}

		private Boolean _AddToiTunes;
		public Boolean AddToiTunes
		{
			get
			{
				return _AddToiTunes;
			}
		}

		private Boolean _ForceConversion;
		public Boolean ForceConversion
		{
			get
			{
				return _ForceConversion;
			}
		}

		private IDirectory _WorkingDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name.Replace(".", FileSystem.DirectorySeperator.ToString()), "WorkingArea"));
		public IDirectory WorkingDirectory
		{
			get
			{
				return _WorkingDirectory;
			}
		}

		// ThreadAvailability for each action.
		private LockableInt CopyMediaToWorkingAreaThreadAvailability = new LockableInt(4);
		private LockableInt ConvertMediaThreadAvailability = new LockableInt(1);
		private LockableInt ExtractExhaustiveMediaDetailsThreadAvailability = new LockableInt(1);
		private LockableInt SaveMediaMetaDataThreadAvailability = new LockableInt(4);
		private LockableInt RenameMediaToCleanNameThreadAvailability = new LockableInt(4);
		private LockableInt AddMediaToiTunesThreadAvailability = new LockableInt(1);
		private LockableInt DeleteMediaThreadAvailability = new LockableInt(10);
		private LockableInt MoveMediaToOutputDirectoryThreadAvailability = new LockableInt(4);

		public Organiser (IDirectory OutputDirectory, Boolean AddToiTunes, Boolean ForceConversion)
		{
			// Setup folders.
			this._OutputDirectory = OutputDirectory;
			this._AddToiTunes = AddToiTunes;
			this._ForceConversion = ForceConversion;

			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}
		}

		public void Organise(IEnumerable<IMedia> Medias)
		{
			Parallel.ForEach(Medias, Media=>
			{
				Log.WriteLine("Organising {0}.", Media.MediaFile.FullName);
				Organise(Media);
				Log.WriteLine("Organised {0}.", Media.MediaFile.FullName);
			});
		}

		public void Organise(IMedia Media)
		{
			// Copy to working area.
			Helper.RunWhenThreadAvailable(CopyMediaToWorkingAreaThreadAvailability, 1, () =>
			{
				CopyMediaToWorkingArea(Media);
			});

			// Convert if required.
			if(ForceConversion || Media.RequiresConversion)
			{
				Helper.RunWhenThreadAvailable(ConvertMediaThreadAvailability, 1, () =>
				{
					ConvertMedia(Media);
				});
			}

			// Extract media details exhaustivly.
			Helper.RunWhenThreadAvailable(ExtractExhaustiveMediaDetailsThreadAvailability, 1, () =>
			{
				ExtractExhaustiveMediaDetails(Media);
			});

			// Save media meta data.
			Helper.RunWhenThreadAvailable(SaveMediaMetaDataThreadAvailability, 1, () =>
			{
				SaveMediaMetaData(Media);
			});

			// Rename media.
			Helper.RunWhenThreadAvailable(RenameMediaToCleanNameThreadAvailability, 1, () =>
			{
				RenameMediaToCleanFileName(Media);
			});

			// Add to iTunes.
			if(AddToiTunes)
			{
				Helper.RunWhenThreadAvailable(AddMediaToiTunesThreadAvailability, 1, () =>
				{
					AddMediaToiTunes(Media);
				});
			}

			// If output directory not provided, delete file. Otherwise move to output directory.
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
			IFile OrganisedMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.OrganisedMediaFile.Name));
			Media.MediaFile.MoveTo(OrganisedMediaFile.FullName, true);
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
			IFile OrganisedFile = new File(FileSystem.PathCombine(OutputDirectory.FullName, Media.OrganisedMediaFile.ToString()));
			if(!OrganisedFile.Directory.Exists)
			{
				OrganisedFile.Directory.Create();
			}
			Media.MediaFile.MoveTo(OrganisedFile.FullName);
			Log.WriteLine("Copied media to output directory. {0}", Media.MediaFile.FullName);
		}
	}
}

