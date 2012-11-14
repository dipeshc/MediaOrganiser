using MediaOrganiser.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Logger;

namespace MediaOrganiser.Organisers
{
	public class Organiser
	{
		private IFileSystem fileSystem = new FileSystem();
		public DirectoryInfoBase WorkingDirectory;

		public TextWriter StdOut
		{
			get { return Logger.Log("Organiser").StdOut; }
			set { Logger.Log("Organiser").StdOut = value; }
		}
		public TextWriter StdErr
		{
			get { return Logger.Log("Organiser").StdErr; }
			set { Logger.Log("Organiser").StdErr = value; }
		}

		public void Organise(IMedia Media, DirectoryInfoBase OutputDirectory, Boolean ForceConversion)
		{
			// Create working directory.
			WorkingDirectory = fileSystem.DirectoryInfo.FromDirectoryName(fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), "WorkingArea"));

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
			var WorkingAreaMediaFile = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(WorkingDirectory.FullName, Media.MediaFile.Name));

			// Copy the media and then assign the new file to the media.
			Media.MediaFile.CopyTo(WorkingAreaMediaFile.FullName, true);
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
			var OrganisedMediaFile = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(WorkingDirectory.FullName, Media.OrganisedMediaFile.Name));
			// Delete the file it already exists.
			if(OrganisedMediaFile.Exists)
			{
				OrganisedMediaFile.Delete();
			}
			Media.MediaFile.MoveTo(OrganisedMediaFile.FullName);
			Logger.Log("Organiser").StdOut.WriteLine("Renamed media. {0}", Media.MediaFile.FullName);
		}

		private void MoveMediaToOutputDirectory(IMedia Media, DirectoryInfoBase OutputDirectory)
		{
			var OrganisedFile = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(OutputDirectory.FullName, Media.OrganisedMediaFile.Name));
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

