using MediaOrganiser.Convertor.HandBrake;
using System;
using System.IO.Abstractions;

namespace MediaOrganiser.Convertor
{
	public static class Convertor
	{ 
		public static bool Convert(FileInfoBase inputFile, FileInfoBase outputFile)
		{
			var escapedInputFullName = inputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(string.Format("-i \"{0}\" -o \"{1}\" -e x264  -q 20.0 -r 30 --pfr  -a 1,1 -E faac,copy:ac3 -B 160,160 -6 dpl2,auto -R Auto,Auto -D 0.0,0.0 -f mp4 -4 --decomb=\"7:2:6:9:1:80\" --loose-anamorphic --modulus 2 -m -x b-adapt=2", escapedInputFullName, outputFile.FullName))==0;
		}


		public static bool ConvertForiPad(FileInfoBase inputFile, FileInfoBase outputFile)
		{
			var escapedInputFullName = inputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(string.Format("-i \"{0}\" -o \"{1}\" --preset=iPad", escapedInputFullName, outputFile.FullName))==0;
		}

		public static bool ConvertForRetina(FileInfoBase inputFile, FileInfoBase outputFile)
		{
			var escapedInputFullName = inputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(string.Format("-i \"{0}\" -o \"{1}\" -e x264  -q 20.0 -r 30 --pfr  -a 1,1 -E faac,copy:ac3 -B 160,160 -6 dpl2,auto -R Auto,Auto -D 0.0,0.0 -f mp4 -4 --width 1280 --decomb=\"7:2:6:9:1:80\" --loose-anamorphic --modulus 2 -m -x b-adapt=2", escapedInputFullName, outputFile.FullName))==0;
		}
	}
}

