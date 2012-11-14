using System;
using System.IO.Abstractions;
using System.Logger;
using System.Reflection;
using System.Diagnostics;
using Mono.Unix.Native;

namespace HandBrake
{
	public static class HandBrake
	{
		private static IFileSystem fileSystem = new FileSystem();

		private static FileInfoBase HandBrakeFile
		{
			get
			{
				// Get HandBrakeCLI file.
				var _HandBrakeFile = fileSystem.FileInfo.FromFileName(fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), "HandBrakeCLI.exe"));

				// Create directory if required.
				if(!_HandBrakeFile.Directory.Exists)
				{
					_HandBrakeFile.Directory.Create();
				}

				// If HandBrake does not exist, then create it.
				if(!_HandBrakeFile.Exists)
				{
					// Create folder if does not exist.
					if(!_HandBrakeFile.Directory.Exists)
					{
						_HandBrakeFile.Directory.Create();
					}

					// Read bytes from assembly and create the HandBrake.
					System.IO.Stream Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Core.Convertor.Externals."+_HandBrakeFile.Name);
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

		public static int Run(String Arguments)
		{
			// Create StartInfo.
			var HandBrakeProcessStartInfo = new ProcessStartInfo();
			HandBrakeProcessStartInfo.FileName = HandBrakeFile.FullName;
			HandBrakeProcessStartInfo.Arguments = Arguments;
			HandBrakeProcessStartInfo.UseShellExecute = false;
			HandBrakeProcessStartInfo.RedirectStandardOutput = true;
			HandBrakeProcessStartInfo.RedirectStandardError = true;

			// Run.
			using(var HandBrakeProcess = Process.Start(HandBrakeProcessStartInfo))
			{
				// Write StdOut.
				HandBrakeProcess.OutputDataReceived += (Sender, E) =>
				{
					Logger.Log("HandBrake").StdOut.Write(E.Data);
				};

				// Write StdErr.
				HandBrakeProcess.ErrorDataReceived += (Sender, E) =>
				{
					Logger.Log("HandBrake").StdErr.Write(E.Data);
				};

				// Wait for process to exit and then return exit code.
				HandBrakeProcess.WaitForExit();
				return HandBrakeProcess.ExitCode;
			}
		}
	}
}

