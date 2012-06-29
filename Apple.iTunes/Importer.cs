using System;
using System.Files;
using System.Diagnostics;
using System.Collections.Generic;

namespace Apple.iTunes
{
	public static class Importer
	{
		public static Boolean Add(IFile File)
		{
			return Add(new List<IFile>{File});
		}

		public static Boolean Add(IEnumerable<IFile> Files)
		{
			foreach(IFile File in Files)
			{
				ImportToiTunes(File);
			}
			return true;
		}

		private static void ImportToiTunes(IFile File)
		{
			String Arguments = String.Format("-e 'tell application \"iTunes\" to add POSIX file \"{0}\"'", File.FullName);

			Process iTunesImportScriptRunner = new Process();
			iTunesImportScriptRunner.StartInfo.FileName = "osascript";
			iTunesImportScriptRunner.StartInfo.Arguments = Arguments;
			iTunesImportScriptRunner.StartInfo.UseShellExecute = false;
			iTunesImportScriptRunner.StartInfo.RedirectStandardOutput = true;
			iTunesImportScriptRunner.Start();
			iTunesImportScriptRunner.StandardOutput.ReadToEnd();
		}
	}
}

