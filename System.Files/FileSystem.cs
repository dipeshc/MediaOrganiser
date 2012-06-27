using System;
using System.IO;

namespace System.Files
{
	public class FileSystem : IFileSystem
	{
		public static String GetTempPath()
		{
			return System.IO.Path.GetTempPath();
		}

		public static String PathCombine(String Path1, String Path2)
		{
			return System.IO.Path.Combine(Path1, Path2);
		}
	}
}

