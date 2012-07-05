using System;

namespace System.Files
{
	public class File : IFile
	{
		private System.IO.FileInfo theFile = null;

		public File (String Path)
		{
			theFile = new System.IO.FileInfo(Path);
		}

		public File (System.IO.FileInfo aFile)
		{
			theFile = aFile;
		}

		public File(IPath Path)
		{
			theFile = new System.IO.FileInfo(Path.FullName);
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

		public IDirectory Directory
		{
			get
			{
				return new Directory(theFile.Directory);
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

			if(Overwrite && PathFile.Exists && PathFile.FullName!=theFile.FullName)
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

