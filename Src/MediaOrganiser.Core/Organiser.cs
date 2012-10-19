using System;
using System.Linq;
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
		public System.IO.TextWriter OutputStream = Console.Out;

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
			OutputStream.WriteLine("Copying media to working area. {0}", Media.MediaFile.FullName);
			// Create file for working area version of media.
			IFile WorkingAreaMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.MediaFile.Name));

			// Copy the media and then assign the new file to the media.
			Media.MediaFile.CopyTo(WorkingAreaMediaFile, true);
			Media.MediaFile = WorkingAreaMediaFile;
			OutputStream.WriteLine("Copied media to working area. {0}", Media.MediaFile.FullName);
		}

		private void ConvertMedia(IMedia Media)
		{
			OutputStream.WriteLine("Starting media conversion. {0}", Media.MediaFile.FullName);
			Media.Convert();
			OutputStream.WriteLine("Converted media. {0}", Media.MediaFile.FullName);
		}

		private void ExtractExhaustiveMediaDetails(IMedia Media)
		{
			OutputStream.WriteLine("Extracting Details Exhaustive. {0}", Media.MediaFile.FullName);
			Media.ExtractDetails(true);
			OutputStream.WriteLine("Extracted Details Exhaustive. {0}", Media.MediaFile.FullName);
		}

		private void SaveMediaMetaData(IMedia Media)
		{
			OutputStream.WriteLine("Saving details. {0}", Media.MediaFile.FullName);
			Media.SaveDetails();
			OutputStream.WriteLine("Saved details. {0}", Media.MediaFile.FullName);
		}

		private void RenameMediaToCleanFileName(IMedia Media)
		{
			OutputStream.WriteLine("Renaming media. {0}", Media.MediaFile.FullName);
			IFile OrganisedMediaFile = new File(FileSystem.PathCombine(WorkingDirectory.FullName, Media.OrganisedMediaFile.Name));
			Media.MediaFile.MoveTo(OrganisedMediaFile.FullName, true);
			OutputStream.WriteLine("Renamed media. {0}", Media.MediaFile.FullName);
		}

		private void MoveMediaToOutputDirectory(IMedia Media, IDirectory OutputDirectory)
		{
			IFile OrganisedFile = new File(FileSystem.PathCombine(OutputDirectory.FullName, Media.OrganisedMediaFile.ToString()));
			if(OrganisedFile.Exists)
			{
				OutputStream.WriteLine("Media file already exists. Will not overwriting. {0}", Media.MediaFile.FullName);
				return;
			}

			OutputStream.WriteLine("Copying media to output directory. {0}", Media.MediaFile.FullName);
			if(!OrganisedFile.Directory.Exists)
			{
				OrganisedFile.Directory.Create();
			}
			Media.MediaFile.MoveTo(OrganisedFile.FullName);
			OutputStream.WriteLine("Copied media to output directory. {0}", Media.MediaFile.FullName);
		}
	}
}

