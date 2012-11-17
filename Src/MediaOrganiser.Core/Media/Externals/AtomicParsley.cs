using System;
using System.IO.Abstractions;
using System.Linq;
using System.Logger;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix.Native;

namespace MediaOrganiser.Media.AtomicParsley
{
	public static class AtomicParsley
	{
		private static IFileSystem _fileSystem = new FileSystem();
		private static Regex _atomDetailRegex = new Regex("Atom \"(.*)\" contains: (.*)");

		private static FileInfoBase _atomicParsleyFile
		{
			get
			{
				// Get AtomicParsely file.
				var atomicParsleyFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "MediaOrganiser" + _fileSystem.Path.DirectorySeparatorChar + "AtomicParsley.exe"));

				// Create directory if required.
				if(!atomicParsleyFile.Directory.Exists)
				{
					atomicParsleyFile.Directory.Create();
				}

				// If AtomicParsely does not exist, then create it.
				if(!atomicParsleyFile.Exists)
				{
					// Create folder if does not exist.
					if(!atomicParsleyFile.Directory.Exists)
					{
						atomicParsleyFile.Directory.Create();
					}

					// Read bytes from assembly and create the file.
					var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Core.Media.Externals."+atomicParsleyFile.Name);
					var bytes = new byte[(int)stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					_fileSystem.File.WriteAllBytes(atomicParsleyFile.FullName, bytes);

					// Set permissions.
					Syscall.chmod(atomicParsleyFile.FullName, FilePermissions.S_IRWXU);
				}

				// Return.
				return atomicParsleyFile;
			}
		}

		public static int Run(string arguments, out string output)
		{
			// Create StartInfo.
			var atomicParsleyProcessStartInfo = new ProcessStartInfo();
			atomicParsleyProcessStartInfo.FileName = _atomicParsleyFile.FullName;
			atomicParsleyProcessStartInfo.Arguments = arguments;
			atomicParsleyProcessStartInfo.UseShellExecute = false;
			atomicParsleyProcessStartInfo.RedirectStandardOutput = true;
			atomicParsleyProcessStartInfo.RedirectStandardError = true;

			// Run.
			using(var atomicParsleyProcess = Process.Start(atomicParsleyProcessStartInfo))
			{
				// Write StdOut.
				var localOutput = output.ToString();
				atomicParsleyProcess.OutputDataReceived += (Sender, E) =>
				{
					localOutput += E.Data;
					Logger.Log("AtomicParsley").StdOut.Write(E.Data);
				};
				
				// Write StdErr.
				atomicParsleyProcess.ErrorDataReceived += (Sender, E) =>
				{
					Logger.Log("AtomicParsley").StdErr.Write(E.Data);
				};
				
				// Wait for process to exit and then assign Output to LocalOuput.
				atomicParsleyProcess.WaitForExit();
				output = localOutput;

				// Return exit code.
				return atomicParsleyProcess.ExitCode;
			}
		}

		public static bool ExtractDetails(string filePath, out Dictionary<string, string> details)
		{
			var output = "";
			if(Run(string.Format("\"{0}\" -t", filePath), out output)!=0)
			{
				details = null;
				return false;
			}

			// Parse out details.
			var localDetails = new Dictionary<string, string>();
			output.Split('\n').ToList().ForEach(Line =>
			{
				// Use regex to extract out details.
				Match Match = _atomDetailRegex.Match(Line);
				if(Match.Success)
				{
					localDetails.Add(Match.Groups[1].Value, Match.Groups[2].Value);
				}
			});
			details = localDetails;

			return true;
		}

		public static bool SetDetails(string filePath, string showName, int? seasonNumber, int? episodeNumber, string episodeName, DateTime? airedDate, string overview, string tvNetwork, IEnumerable<String> artworkPaths=null)
		{
			var arguments = String.Format(
				"\"{0}\" --overWrite --stik \"TV Show\" --TVShowName \"{1}\" --TVSeasonNum \"{2}\" --TVEpisodeNum \"{3}\" --tracknum \"{3}\" --title \"{4}\" --year \"{5}\" --description \"{6}\" --TVNetwork \"{7}\" --artwork REMOVE_ALL",
				filePath,
				showName.Replace("\\", "\\\\").Replace("\"", "\\\""),
				seasonNumber,
				episodeNumber,
				episodeName==null?"":episodeName.Replace("\\", "\\\\").Replace("\"", "\\\""),
				(airedDate==null?"":((DateTime)airedDate).ToString("u").Replace(" ", "T")),
				overview==null?"":overview.Replace("\\", "\\\\").Replace("\"", "\\\""),
				tvNetwork==null?"":tvNetwork.Replace("\\", "\\\\").Replace("\"", "\\\""));

			if(artworkPaths!=null)
			{
				foreach(var artworkPath in artworkPaths)
				{
					arguments += String.Format(" --artwork \"{0}\"", artworkPath);
				}
			}

			String output = "";
			return Run(arguments, out output)==0;
		}
	}
}

