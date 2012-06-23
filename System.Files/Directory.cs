using System;
using System.IO;
using System.Collections.Generic;

namespace System.Files
{
	public class Directory : IDirectory
	{
		DirectoryInfo theDirectory = null;

		public Directory (String Path)
		{
			theDirectory = new DirectoryInfo(Path);
		}

		public Directory (DirectoryInfo aDirectory)
		{
			theDirectory = aDirectory;
		}

		public String Name
		{
			get
			{
				return theDirectory.Name;
			}
		}

		public String FullName
		{
			get
			{
				return theDirectory.FullName;
			}
		}

		public Boolean Exists
		{
			get
			{
				return theDirectory.Exists;
			}
		}

		public void Create()
		{
			theDirectory.Create();
		}

		public IEnumerable<IFile> GetFilesInTopDirectoryOnly(String SearchPattern)
		{
			IList<IFile> Files = new List<IFile>();
			foreach(FileInfo FileInfo in theDirectory.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly))
			{
				Files.Add(new File(FileInfo));
			}
			return Files;
		}

		public IEnumerable<IFile> GetFilesInAllDirectories(String SearchPattern)
		{
			IList<IFile> Files = new List<IFile>();
			foreach(FileInfo FileInfo in theDirectory.GetFiles(SearchPattern, SearchOption.AllDirectories))
			{
				Files.Add(new File(FileInfo));
			}
			return Files;
		}
	}
}

