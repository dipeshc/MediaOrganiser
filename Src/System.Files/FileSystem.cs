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

		public static String PathCombine(params String[] Paths)
		{
			if(Paths.Length==0)
			{
				return "";
			}

			if(Paths.Length==1)
			{
				return Paths[0];
			}

			String Path = Paths[0];

			for(int I=1; I!=Paths.Length; ++I)
			{
				Path = System.IO.Path.Combine(Path, Paths[I]);
			}
			return Path;
		}

		public static Char DirectorySeperator
		{
			get
			{
				return System.IO.Path.DirectorySeparatorChar;
			}
		}
	}
}

