using MediaOrganiser.HandBrake;
using System;
using System.IO.Abstractions;

namespace MediaOrganiser.Convertor
{
	public static class Convertor
	{
		public static int ConvertForiPad(FileInfoBase InputFile, FileInfoBase OutputFile)
		{
			String EscapedInputFullName = InputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(String.Format("-i \"{0}\" -o \"{1}\" --preset=iPad", EscapedInputFullName, OutputFile.FullName));
		}

		public static int ConvertForRetina(FileInfoBase InputFile, FileInfoBase OutputFile)
		{
			String EscapedInputFullName = InputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(String.Format("-i \"{0}\" -o \"{1}\" -e x264  -q 20.0 -r 30 --pfr  -a 1,1 -E faac,copy:ac3 -B 160,160 -6 dpl2,auto -R Auto,Auto -D 0.0,0.0 -f mp4 -4 --width 1280 --decomb=\"7:2:6:9:1:80\" --loose-anamorphic --modulus 2 -m -x b-adapt=2", EscapedInputFullName, OutputFile.FullName));
		}
	}
}

