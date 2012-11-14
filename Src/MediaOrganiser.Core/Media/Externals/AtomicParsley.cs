using System;
using System.IO.Abstractions;
using System.Linq;
using System.Logger;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix.Native;

namespace AtomicParsley
{
	public static class AtomicParsley
	{
		private static IFileSystem fileSystem = new FileSystem();
		private static Regex AtomDetailRegex = new Regex("Atom \"(.*)\" contains: (.*)");

		private static FileInfoBase AtomicParsleyFile
		{
			get
			{
				// Get AtomicParsely file.
				var _AtomicParsleyFile = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), "MediaOrganiser" + fileSystem.Path.DirectorySeparatorChar + "AtomicParsley.exe"));

				// Create directory if required.
				if(!_AtomicParsleyFile.Directory.Exists)
				{
					_AtomicParsleyFile.Directory.Create();
				}

				// If AtomicParsely does not exist, then create it.
				if(!_AtomicParsleyFile.Exists)
				{
					// Create folder if does not exist.
					if(!_AtomicParsleyFile.Directory.Exists)
					{
						_AtomicParsleyFile.Directory.Create();
					}

					// Read bytes from assembly and create the file.
					System.IO.Stream Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Core.Media.Externals."+_AtomicParsleyFile.Name);
					byte[] Bytes = new byte[(int)Stream.Length];
					Stream.Read(Bytes, 0, Bytes.Length);
					System.IO.File.WriteAllBytes(_AtomicParsleyFile.FullName, Bytes);

					// Set permissions.
					Syscall.chmod(_AtomicParsleyFile.FullName, FilePermissions.S_IRWXU);
				}

				// Return.
				return _AtomicParsleyFile;
			}
		}

		public static int Run(String Arguments, out string Output)
		{
			// Create StartInfo.
			var AtomicParsleyProcessStartInfo = new ProcessStartInfo();
			AtomicParsleyProcessStartInfo.FileName = AtomicParsleyFile.FullName;
			AtomicParsleyProcessStartInfo.Arguments = Arguments;
			AtomicParsleyProcessStartInfo.UseShellExecute = false;
			AtomicParsleyProcessStartInfo.RedirectStandardOutput = true;
			AtomicParsleyProcessStartInfo.RedirectStandardError = true;

			// Run.
			using(var AtomicParsleyProcess = Process.Start(AtomicParsleyProcessStartInfo))
			{
				// Write StdOut.
				var LocalOutput = Output.ToString();
				AtomicParsleyProcess.OutputDataReceived += (Sender, E) =>
				{
					LocalOutput += E.Data;
					Logger.Log("AtomicParsley").StdOut.Write(E.Data);
				};
				
				// Write StdErr.
				AtomicParsleyProcess.ErrorDataReceived += (Sender, E) =>
				{
					Logger.Log("AtomicParsley").StdErr.Write(E.Data);
				};
				
				// Wait for process to exit and then assign Output to LocalOuput.
				AtomicParsleyProcess.WaitForExit();
				Output = LocalOutput;

				// Return exit code.
				return AtomicParsleyProcess.ExitCode;
			}
		}

		public static Dictionary<String, String> ExtractDetails(String FilePath)
		{
			Dictionary<String, String> Details = new Dictionary<String, String>();

			string Output = "";
			if(Run(String.Format("\"{0}\" -t", FilePath), out Output)!=0)
			{
				return Details;
			}

			Output.Split('\n').ToList().ForEach(Line =>
			{
				// Use regex to extract out details.
				Match Match = AtomDetailRegex.Match(Line);
				if(Match.Success)
				{
					Details.Add(Match.Groups[1].Value, Match.Groups[2].Value);
				}
			});

			return Details;
		}

		public static void SetDetails(String FilePath, String ShowName, Int32? SeasonNumber, Int32? EpisodeNumber, String EpisodeName, DateTime? AiredDate, String Overview, String TVNetwork, IEnumerable<String> ArtworkPaths=null)
		{
			String Arguments = String.Format(
				"\"{0}\" --overWrite --stik \"TV Show\" --TVShowName \"{1}\" --TVSeasonNum \"{2}\" --TVEpisodeNum \"{3}\" --tracknum \"{3}\" --title \"{4}\" --year \"{5}\" --description \"{6}\" --TVNetwork \"{7}\" --artwork REMOVE_ALL",
				FilePath,
				ShowName.Replace("\\", "\\\\").Replace("\"", "\\\""),
				SeasonNumber,
				EpisodeNumber,
				EpisodeName==null?"":EpisodeName.Replace("\\", "\\\\").Replace("\"", "\\\""),
				(AiredDate==null?"":((DateTime)AiredDate).ToString("u").Replace(" ", "T")),
				Overview==null?"":Overview.Replace("\\", "\\\\").Replace("\"", "\\\""),
				TVNetwork==null?"":TVNetwork.Replace("\\", "\\\\").Replace("\"", "\\\""));

			if(ArtworkPaths!=null)
			{
				foreach(String ArtworkPath in ArtworkPaths)
				{
					Arguments += String.Format(" --artwork \"{0}\"", ArtworkPath);
				}
			}

			String Output = "";
			Run(Arguments, out Output);
		}
	}
}

