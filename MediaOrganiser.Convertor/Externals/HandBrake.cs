using System;
using System.Files;
using System.Reflection;
using System.Diagnostics;

namespace HandBrake
{
	public static class HandBrake
	{
		private static IFile HandBrakeFile
		{
			get
			{
				IFile _HandBrakeFile = new File(FileSystem.PathCombine(FileSystem.GetTempPath(), "MediaOrganiser", "HandBrakeCLI.exe"));
				if(!_HandBrakeFile.Exists)
				{
					System.IO.Stream Stream = typeof(HandBrake).Assembly.GetManifestResourceStream("MediaOrganiser.Convertor.Externals.HandBrakeCLI");
					byte[] Bytes = new byte[(int)Stream.Length];
					Stream.Read(Bytes, 0, Bytes.Length);
					System.IO.File.WriteAllBytes(_HandBrakeFile.FullName, Bytes);
					Mono.Posix.Syscall.chmod (_HandBrakeFile.FullName, Mono.Posix.FileMode.S_IXUSR);
				}
				return _HandBrakeFile;
			}
		}

		public static String Run(String Arguments)
		{
			Process HandBrake = new Process();
			HandBrake.StartInfo.FileName = HandBrakeFile.FullName;
			HandBrake.StartInfo.Arguments = Arguments;
			HandBrake.StartInfo.UseShellExecute = false;
			HandBrake.StartInfo.RedirectStandardOutput = true;
			HandBrake.Start();
			return HandBrake.StandardOutput.ReadToEnd();
		}
	}
}

