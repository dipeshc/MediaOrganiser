using System;

namespace System.Files
{
	public interface IPath
	{
		String Name { get; }
		String FullName { get; }
		Boolean IsFile { get; }
		Boolean IsDirectory { get; }
	}
}

