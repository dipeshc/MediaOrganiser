using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AtomicParsley
{
	public static class AtomicParsley
	{
		private const string AtomicParsleyFilePath = @"./AtomicParsley";
		private static Regex DetailRegex = new Regex("Atom \"(.*)\" contains: (.*)");

		public static String Run(String Arguments)
		{
			Process AtomicParsley = new Process();
			AtomicParsley.StartInfo.FileName = AtomicParsleyFilePath;
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
				Match Match = DetailRegex.Match(Line);
				if(Match.Success)
				{
					Details.Add(Match.Groups[1].Value, Match.Groups[2].Value);
				}
			});

			return Details;
		}

		public static void SetDetails(String FilePath, String ShowName, Int32? SeasonNumber, Int32? EpisodeNumber, String EpisodeName, DateTime? AiredDate, String Overview, String TVNetwork, String ArtworkPath=null)
		{
			String Arguments = String.Format(
				"\"{0}\" --overWrite --stik \"TV Show\" --TVShowName \"{1}\" --TVSeasonNum \"{2}\" --TVEpisodeNum \"{3}\" --tracknum \"{3}\" --title \"{4}\" --year \"{5}\" --description \"{6}\" --TVNetwork \"{7}\"",
				FilePath,
				ShowName,
				SeasonNumber,
				EpisodeNumber,
				EpisodeName,
				(AiredDate==null?"":((DateTime)AiredDate).ToString("u").Replace(" ", "T")),
				Overview,
				TVNetwork);

			if(ArtworkPath!=null)
			{
				Arguments += String.Format(" --artwork \"{0}\"", ArtworkPath);
			}

			String Output = Run(Arguments);

			Console.WriteLine(Output);
		}
	}
}

