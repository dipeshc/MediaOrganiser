using System;
using System.IO.Abstractions;
using System.Logger;
using System.Reflection;
using System.Diagnostics;
using Mono.Unix.Native;

namespace MediaOrganiser.Convertor.HandBrake
{
	public static class HandBrake
	{
		private static IFileSystem _fileSystem = new FileSystem();

		private static FileInfoBase _handBrakeFile
		{
			get
			{
				// Get HandBrakeCLI file.
				var handBrakeFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), "HandBrakeCLI.exe"));

				// Create directory if required.
				if(!handBrakeFile.Directory.Exists)
				{
					handBrakeFile.Directory.Create();
				}

				// If HandBrake does not exist, then create it.
				if(!handBrakeFile.Exists)
				{
					// Create folder if does not exist.
					if(!handBrakeFile.Directory.Exists)
					{
						handBrakeFile.Directory.Create();
					}

					// Read bytes from assembly and create the HandBrake.
					var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MediaOrganiser.Core.Convertor.Externals." + handBrakeFile.Name);
					var bytes = new byte[(int)stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					_fileSystem.File.WriteAllBytes(handBrakeFile.FullName, bytes);

					// Set permissions.
					Syscall.chmod(handBrakeFile.FullName, FilePermissions.S_IRWXU);
				}

				// Return.
				return handBrakeFile;
			}
		}

		public static int Run(string arguments)
		{
			// Create StartInfo.
			var handBrakeProcessStartInfo = new ProcessStartInfo();
			handBrakeProcessStartInfo.FileName = _handBrakeFile.FullName;
			handBrakeProcessStartInfo.Arguments = arguments;
			handBrakeProcessStartInfo.UseShellExecute = false;
			handBrakeProcessStartInfo.RedirectStandardOutput = true;
			handBrakeProcessStartInfo.RedirectStandardError = true;

			// Run.
			using(var HandBrakeProcess = Process.Start(handBrakeProcessStartInfo))
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

