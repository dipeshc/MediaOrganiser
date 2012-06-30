using System;
using System.IO;

namespace System.Files
{
	public class File : IFile
	{
		private FileInfo theFile = null;

		public File (String Path)
		{
			theFile = new FileInfo(Path);
		}

		public File (FileInfo aFile)
		{
			theFile = aFile;
		}

		public File(IPath Path)
		{
			theFile = new FileInfo(Path.FullName);
		}

		public String Name
		{
			get
			{
				return theFile.Name;
			}
		}

		public String FullName
		{
			get
			{
				return theFile.FullName;
			}
		}

		public String Extension
		{
			get
			{
				return theFile.Extension;
			}
		}

		public String NameWithoutExtension
		{
			get
			{
				return theFile.Name.Substring(0, theFile.Name.LastIndexOf(theFile.Extension));
			}
		}

		public String FullNameWithoutExtension
		{
			get
			{
				return theFile.FullName.Substring(0, theFile.FullName.LastIndexOf(theFile.Extension));
			}
		}

		public Boolean Exists
		{
			get
			{
				return theFile.Exists;
			}
		}

		public void Create()
		{
			theFile.Create();
		}

		public void CopyTo(IFile CopyToFile, Boolean Overwrite=false)
		{
			theFile.CopyTo(CopyToFile.FullName, Overwrite);
		}

		public void MoveTo(String Path, Boolean Overwrite=false)
		{
			File PathFile = new File(Path);

			if(Overwrite && PathFile.Exists)
			{
				PathFile.Delete();
			}

			theFile.MoveTo(PathFile.FullName);
		}

		public void Delete()
		{
			theFile.Delete();
		}
	}
}

