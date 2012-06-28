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
				String AppleScript = @"osascript -e 'tell application ""iTunes"" to add POSIX file ""{0}""'";
				Process.Start(AppleScript);
			}
			return true;
		}

	}
}

