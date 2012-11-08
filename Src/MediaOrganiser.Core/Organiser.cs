using System;
using System.Linq;
using System.Logger;
using System.Files;
using System.Files.Interfaces;
using System.Collections;
using System.Collections.Generic;
using MediaOrganiser.Media;

namespace MediaOrganiser.Organisers
{
	public class Organiser
	{
		public IDirectory WorkingDirectory = new Directory(FileSystem.PathCombine(FileSystem.GetTempPath(), "WorkingArea"));

		public System.IO.TextWriter StdOut
		{
			get { return Logger.Log("Organiser").StdOut; }
			set { Logger.Log("Organiser").StdOut = value; }
		}
		public System.IO.TextWriter StdErr
		{
			get { return Logger.Log("Organiser").StdErr; }
			set { Logger.Log("Organiser").StdErr = value; }
		}

		public void Organise(IMedia Media, IDirectory OutputDirectory, Boolean ForceConversion)
		{
			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}

			// Copy to working area.
			CopyMediaToWorkingArea(Media);

			// Convert if required.
			if(ForceConversion || Media.RequiresConversion)
			{
				ConvertMedia(Media);
			}

			// Extract media details exhaustivly.
			ExtractExhaustiveMediaDetails(Media);

			// Save media meta data.
			SaveMediaMetaData(Media);

			// Rename media.
			RenameMediaToCleanFileName(Media);

			// If output directory not provided, delete file. Otherwise move to output directory.
			MoveMediaToOutputDirectory(Media, OutputDirectory);
		}

		private void CopyMediaToWorkingArea(IMedia Media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Copying media to working area. {0}", Media.MediaFile.FullName);
			// Create file for working area version of media.
			IFile WorkingAreaMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.MediaFile.Name));

			// Copy the media and then assign the new file to the media.
			Media.MediaFile.CopyTo(WorkingAreaMediaFile, true);
			Media.MediaFile = WorkingAreaMediaFile;
			Logger.Log("Organiser").StdOut.WriteLine("Copied media to working area. {0}", Media.MediaFile.FullName);
		}

		private void ConvertMedia(IMedia Media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Starting media conversion. {0}", Media.MediaFile.FullName);
			Media.Convert();
			Logger.Log("Organiser").StdOut.WriteLine("Converted media. {0}", Media.MediaFile.FullName);
		}

		private void ExtractExhaustiveMediaDetails(IMedia Media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Extracting Details Exhaustive. {0}", Media.MediaFile.FullName);
			Media.ExtractDetails(true);
			Logger.Log("Organiser").StdOut.WriteLine("Extracted Details Exhaustive. {0}", Media.MediaFile.FullName);
		}

		private void SaveMediaMetaData(IMedia Media)
		{
			Logger.Log().StdOut.WriteLine("Saving details. {0}", Media.MediaFile.FullName);
			Media.SaveDetails();
			Logger.Log().StdOut.WriteLine("Saved details. {0}", Media.MediaFile.FullName);
		}

		private void RenameMediaToCleanFileName(IMedia Media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Renaming media. {0}", Media.MediaFile.FullName);
			IFile OrganisedMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.OrganisedMediaFile.Name));
			Media.MediaFile.MoveTo(OrganisedMediaFile.FullName, true);
			Logger.Log("Organiser").StdOut.WriteLine("Renamed media. {0}", Media.MediaFile.FullName);
		}

		private void MoveMediaToOutputDirectory(IMedia Media, IDirectory OutputDirectory)
		{
			IFile OrganisedFile = new File(FileSystem.PathCombine(OutputDirectory.FullName, Media.OrganisedMediaFile.ToString()));
			if(OrganisedFile.Exists)
			{
				Logger.Log("Organiser").StdOut.WriteLine("Media file already exists. Will not overwriting. {0}", Media.MediaFile.FullName);
				return;
			}

			Logger.Log("Organiser").StdOut.WriteLine("Copying media to output directory. {0}", Media.MediaFile.FullName);
			if(!OrganisedFile.Directory.Exists)
			{
				OrganisedFile.Directory.Create();
			}
			Media.MediaFile.MoveTo(OrganisedFile.FullName);
			Logger.Log("Organiser").StdOut.WriteLine("Copied media to output directory. {0}", Media.MediaFile.FullName);
		}
	}
}

