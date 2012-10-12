using System;

namespace System.Files
{
	public interface IFile
	{
		String Name { get; }
		String FullName { get; }
		String Extension { get; }
		String NameWithoutExtension { get; }
		String FullNameWithoutExtension { get; }
		Boolean Exists { get ; }
		long Length { get; }
		IDirectory Directory { get; }
		void Create();
		void CopyTo(IFile CopyToFile, Boolean Overwrite=false);
		void MoveTo(String Path, Boolean Overwrite=false);
		void Delete();
	}
}
