using System;
using System.Diagnostics;

namespace HandBrake
{
	public static class HandBrake
	{
		private const string HandBrakeFilePath = @"./HandBrakeCLI";

		public static String Run(String Arguments)
		{
			Process HandBrake = new Process();
			HandBrake.StartInfo.FileName = HandBrakeFilePath;
			HandBrake.StartInfo.Arguments = Arguments;
			HandBrake.StartInfo.UseShellExecute = false;
			HandBrake.StartInfo.RedirectStandardOutput = true;
			HandBrake.Start();
			return HandBrake.StandardOutput.ReadToEnd();
		}
	}
}

