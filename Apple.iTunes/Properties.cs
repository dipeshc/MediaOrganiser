using System;
using System.Files;

namespace Apple.iTunes
{
	public static class Properties
	{
		private static String iTunesMediaPath
		{
			get
			{
				String HomePath  = System.Environment.GetEnvironmentVariable("HOME");
				return System.IO.Path.GetFullPath(HomePath+"/Music/iTunes/iTunes Media");
			}
		}

		public static IDirectory RootMediaDirectory
		{
			get
			{
				System.IO.Path.GetFullPath(iTunesMediaPath);
				return new Directory(iTunesMediaPath);
			}
		}

		public static IDirectory MusicMediaDirectory
		{
			get
			{
				return new Directory(iTunesMediaPath + "/Music");
			}
		}

		public static IDirectory MovieMediaDirectory
		{
			get
			{
				return new Directory(iTunesMediaPath + "/Movies");
			}
		}

		public static IDirectory TVShowMediaDirectory
		{
			get
			{
				return new Directory(iTunesMediaPath + "/TV Shows");
			}
		}
	}
}