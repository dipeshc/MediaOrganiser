using MediaOrganiser.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Logger;

namespace MediaOrganiser.Console.Organisers
{
	public enum OrganiserConversionOptions
	{
		Default,
		Force,
		Skip
	}

	public class Organiser
	{
		private IFileSystem _fileSystem = new FileSystem();
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

		public void Organise(IMedia media, DirectoryInfoBase outputDirectory, OrganiserConversionOptions conversionOption, bool strictSeason)
		{
			// Create working directory.
			WorkingDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "WorkingArea"));

			// Create working directory if it does not exist.
			if(!WorkingDirectory.Exists)
			{
				WorkingDirectory.Create();
			}

			// Copy to working area.
			CopyMediaToWorkingArea(media);

			// Convert if required.
			if(conversionOption == OrganiserConversionOptions.Force)
			{
				Logger.Log("Organiser").StdOut.WriteLine("Conversion set to \"force\". Will convert. {0}", media.MediaFile.FullName);
				ConvertMedia(media);
			}
			else if(media.RequiresConversion)
			{
				if(conversionOption == OrganiserConversionOptions.Skip)
				{
					Logger.Log("Organiser").StdOut.WriteLine("Media requires conversion. Conversion set to \"skip\", skipping conversion. {0}", media.MediaFile.FullName);
				}
				else
				{
					Logger.Log("Organiser").StdOut.WriteLine("Media requires conversion. Will convert. {0}", media.MediaFile.FullName);
					ConvertMedia(media);
				}
			}

			// Extract media details exhaustivly.
			ExtractExhaustiveMediaDetails(media, strictSeason);

			// Save media meta data.
			var saveResponse = SaveMediaMetaData(media);
			if(!saveResponse)
			{
				if(conversionOption == OrganiserConversionOptions.Skip)
				{
					Logger.Log("Organiser").StdOut.WriteLine("Unable to save metadata. Conversion set to \"skip\", skipping conversion. {0}", media.MediaFile.FullName);
				}
				else
				{
					Logger.Log("Organiser").StdOut.WriteLine("Unable to save metadata. Will convert. {0}", media.MediaFile.FullName);
					ConvertMedia(media);
					SaveMediaMetaData(media);
				}
			}

			// Rename media.
			RenameMediaToCleanFileName(media);

			// If output directory not provided, delete file. Otherwise move to output directory.
			MoveMediaToOutputDirectory(media, outputDirectory);
		}

		private bool CopyMediaToWorkingArea(IMedia media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Copying media to working area. {0}", media.MediaFile.FullName);
			// Create file for working area version of media.
			var WorkingAreaMediaFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(WorkingDirectory.FullName, media.MediaFile.Name));
			// Copy the media and then assign the new file to the media.
			media.MediaFile.CopyTo(WorkingAreaMediaFile.FullName, true);
			media.MediaFile = WorkingAreaMediaFile;
			Logger.Log("Organiser").StdOut.WriteLine("Copied media to working area. {0}", media.MediaFile.FullName);
			return true;
		}

		private bool ConvertMedia(IMedia media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Starting media conversion. {0}", media.MediaFile.FullName);
			if(media.Convert())
			{
				Logger.Log("Organiser").StdOut.WriteLine("Converted media. {0}", media.MediaFile.FullName);
				return true;
			}
			Logger.Log("Organiser").StdOut.WriteLine("Unable to convert media. {0}", media.MediaFile.FullName);
			return false;
		}

		private bool ExtractExhaustiveMediaDetails(IMedia media, bool strictSeason)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Extracting Details Exhaustive. {0}", media.MediaFile.FullName);
			var response = media.ExtractDetails(true, strictSeason);
			Logger.Log("Organiser").StdOut.WriteLine("Extracted Details Exhaustive. {0}", media.MediaFile.FullName);
			return response;
		}

		private bool SaveMediaMetaData(IMedia media)
		{
			Logger.Log().StdOut.WriteLine("Saving details. {0}", media.MediaFile.FullName);
			if(media.SaveDetails())
			{
				Logger.Log().StdOut.WriteLine("Saved details. {0}", media.MediaFile.FullName);
				return true;
			}
			Logger.Log().StdOut.WriteLine("Unable to save details. {0}", media.MediaFile.FullName);
			return false;
		}

		private bool RenameMediaToCleanFileName(IMedia media)
		{
			Logger.Log("Organiser").StdOut.WriteLine("Renaming media. {0}", media.MediaFile.FullName);
			var outputFileName = _fileSystem.Path.GetFileName(media.OrganisedMediaFileOutputPath);
			var OrganisedMediaFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(WorkingDirectory.FullName, outputFileName));
			// Delete the file it already exists.
			if(OrganisedMediaFile.Exists)
			{
				OrganisedMediaFile.Delete();
			}
			media.MediaFile.MoveTo(OrganisedMediaFile.FullName);
			Logger.Log("Organiser").StdOut.WriteLine("Renamed media. {0}", media.MediaFile.FullName);
			return true;
		}

		private bool MoveMediaToOutputDirectory(IMedia media, DirectoryInfoBase outputDirectory)
		{
			// Get the output file location of the media.
			var OrganisedFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(outputDirectory.FullName, media.OrganisedMediaFileOutputPath));
			if(OrganisedFile.Exists)
			{
				Logger.Log("Organiser").StdOut.WriteLine("Media file already exists. Will not overwriting. {0}", media.MediaFile.FullName);
				return false;
			}

			Logger.Log("Organiser").StdOut.WriteLine("Copying media to output directory. {0}", media.MediaFile.FullName);
			if(!OrganisedFile.Directory.Exists)
			{
				OrganisedFile.Directory.Create();
			}
			media.MediaFile.MoveTo(OrganisedFile.FullName);
			Logger.Log("Organiser").StdOut.WriteLine("Copied media to output directory. {0}", media.MediaFile.FullName);
			return true;
		}
	}
}

