using System;
using System.Files;
using System.Files.Interfaces;
using System.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediaOrganiser.Media;
using MediaOrganiser.Finders;
using MediaOrganiser.Organisers;

namespace MediaOrganiser
{
	public class MediaOrganiser
	{
		private static Int32 DaemonModeThreadWaitTimeInMilliseconds = 5000;
		private IOrganiser Organiser;
		private IMediaFinder MediaFinder;

		public MediaOrganiser(IEnumerable<IPath> InputPaths, IEnumerable<IPath> ExcludedPaths, IDirectory OutputDirectory, Boolean AddToiTunes, Boolean ExcludeExisting, Boolean ForceConversion, Boolean Clean)
		{
			// Colsoliate arguments.
			List<IPath> _ExcludedPaths = new List<IPath>(ExcludedPaths);
			if(OutputDirectory!=null && ExcludeExisting)
			{
				_ExcludedPaths.Add(new Path(OutputDirectory.FullName));
			}
			if(AddToiTunes && ExcludeExisting)
			{
				_ExcludedPaths.Add(new Path(Apple.iTunes.Properties.RootMediaDirectory.FullName));
			}

			// Clean.
			if(Clean)
			{
				new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name)).Delete(true);
			}

			// Create organiser and finders.
			Organiser = new Organiser(OutputDirectory, AddToiTunes, ForceConversion);
			MediaFinder = new ShowFinder(InputPaths, _ExcludedPaths);
		}

		public void Execute()
		{
			Organiser.Organise(MediaFinder.Scan());
		}

		public void ExecuteInWatcherMode()
		{
			// Create queue.
			Queue<IMedia> MediaToBeOrganised = new Queue<IMedia>();

			// Create new thread to do inital scan and setup watchers.
			(new Thread(() =>
			{
				MediaFinder.ScanAndWatch((Sender, Media) =>
				{
					Log.WriteLine("Enqueing {0}.", Media.MediaFile.FullName);
					MediaToBeOrganised.Enqueue(Media);
					Log.WriteLine("Enqueued {0}.", Media.MediaFile.FullName);
				});
			})
			{
				Name = "Scan and Watch Thread",
				Priority = ThreadPriority.Normal
			}).Start();

			// Run forever and take off queue as required.
			while(true)
			{
				Log.WriteLine("Media queue length {0}.", MediaToBeOrganised.Count);
				while(MediaToBeOrganised.Count!=0)
				{
					Parallel.For(0, MediaToBeOrganised.Count, LoopNumber =>
					{
						IMedia Media = MediaToBeOrganised.Dequeue();
						Log.WriteLine("Media queue length {0}.", MediaToBeOrganised.Count);
						Log.WriteLine("Organising {0}.", Media.MediaFile.FullName);
						Organiser.Organise(Media);
						Log.WriteLine("Organised {0}.", Media.MediaFile.FullName);
					});
				}
				Log.WriteLine("Waiting...");
				Thread.Sleep(DaemonModeThreadWaitTimeInMilliseconds);
			}
		}
	}
}
