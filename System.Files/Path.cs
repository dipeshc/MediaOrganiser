using System;

namespace System.Files
{
	public class Path : IPath
	{
		private String thePath;

		public Path (String Path)
		{
			this.thePath = Path;
		}

		public String Name
		{
			get
			{
				return System.IO.Path.GetFileName(thePath);
			}
		}

		public String FullName
		{
			get
			{
				return System.IO.Path.GetFullPath(thePath);
			}
		}

		public Boolean IsFile
		{
			get
			{
				return System.IO.File.Exists(thePath);
			}
		}

		public Boolean IsDirectory
		{
			get
			{
				return System.IO.Directory.Exists(thePath);
			}
		}
	}
}

