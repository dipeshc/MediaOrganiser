using System;
using System.Files;
using System.Reflection;
using System.Diagnostics;
using Mono.Unix.Native;

namespace HandBrake
{
	public static class HandBrake
	{
		private static IFile HandBrakeFile
		{
			get
			{
				// Get HandBrakeCLI file.
				IFile _HandBrakeFile = new File(FileSystem.PathCombine(FileSystem.GetTempPath(), Assembly.GetExecutingAssembly().GetName().Name, "HandBrakeCLI.exe"));

				// If HandBrake does not exist, then create it.
				if(!_HandBrakeFile.Exists)
				{
					// Create folder if does not exist.
					if(!_HandBrakeFile.Directory.Exists)
					{
						_HandBrakeFile.Directory.Create();
					}

					// Read bytes from assembly and create the HandBrake.
					System.IO.Stream Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Convertor.Externals."+_HandBrakeFile.NameWithoutExtension);
					byte[] Bytes = new byte[(int)Stream.Length];
					Stream.Read(Bytes, 0, Bytes.Length);
					System.IO.File.WriteAllBytes(_HandBrakeFile.FullName, Bytes);

					// Set permissions.
					Syscall.chmod(_HandBrakeFile.FullName, FilePermissions.S_IRWXU);
				}

				// Return.
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

