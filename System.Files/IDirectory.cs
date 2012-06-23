using System;
using System.Collections.Generic;

namespace System.Files
{
	public interface IDirectory
	{
		String Name { get; }
		String FullName { get; }
		Boolean Exists { get; }
		void Create();
		IEnumerable<IFile> GetFilesInTopDirectoryOnly(String SearchPattern);
		IEnumerable<IFile> GetFilesInAllDirectories(String SearchPattern);
	}
}

