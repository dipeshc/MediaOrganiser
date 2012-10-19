using System;
using System.Linq;
using System.Files;
using System.Files.Interfaces;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix.Native;

namespace AtomicParsley
{
	public static class AtomicParsley
	{
		private static Regex AtomDetailRegex = new Regex("Atom \"(.*)\" contains: (.*)");

		private static IFile AtomicParsleyFile
		{
			get
			{
				// Get AtomicParsely file.
				IFile _AtomicParsleyFile = new File(FileSystem.PathCombine(FileSystem.GetTempPath(), "AtomicParsley.exe"));

				// If AtomicParsely does not exist, then create it.
				if(!_AtomicParsleyFile.Exists)
				{
					// Create folder if does not exist.
					if(!_AtomicParsleyFile.Directory.Exists)
					{
						_AtomicParsleyFile.Directory.Create();
					}

					// Read bytes from assembly and create the file.
					System.IO.Stream Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Media.Externals."+_AtomicParsleyFile.Name);
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

		public static String Run(String Arguments)
		{
			Process AtomicParsley = new Process();
			AtomicParsley.StartInfo.FileName = AtomicParsleyFile.FullName;
			AtomicParsley.StartInfo.Arguments = Arguments;
			AtomicParsley.StartInfo.UseShellExecute = false;
			AtomicParsley.StartInfo.RedirectStandardOutput = true;
			AtomicParsley.Start();
			return AtomicParsley.StandardOutput.ReadToEnd();
		}

		public static Dictionary<String, String> ExtractDetails(String FilePath)
		{
			Dictionary<String, String> Details = new Dictionary<String, String>();

			String Output = Run(String.Format("\"{0}\" -t", FilePath));

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

			String Output = Run(Arguments);

			Console.WriteLine(Output);
		}
	}
}

