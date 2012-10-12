using System;
using System.Collections.Generic;

namespace System.Files
{
	public interface IDirectory
	{
		String Name { get; }
		String FullName { get; }
		Boolean Exists { get; }
		IDirectory Parent { get; }
		void Create();
		void Delete(Boolean Recursive);
		IEnumerable<IDirectory> GetSubdirectories(String SearchPattern);
		IEnumerable<IFile> GetFilesInTopDirectoryOnly(String SearchPattern);
		IEnumerable<IFile> GetFilesInAllDirectories(String SearchPattern);
	}
}
