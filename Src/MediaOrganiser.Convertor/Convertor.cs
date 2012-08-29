using System;
using System.Files;
using HandBrake;

namespace MediaOrganiser.Convertor
{
	public static class Convertor
	{
		public enum Quality
		{
			Default,
			iPhone,
			iPad,
			Retina
		}

		public static String Convert(IFile InputFile, IFile OutputFile, Quality Quality)
		{
			String EscapedInputFullName = InputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			String Command = String.Format("-i \"{0}\" -o \"{1}\" ", EscapedInputFullName, OutputFile.FullName);

			switch(Quality)
			{
				case Quality.Retina:
					Command += "-e x264  -q 20.0 -r 30 --pfr  -a 1,1 -E faac,copy:ac3 -B 160,160 -6 dpl2,auto -R Auto,Auto -D 0.0,0.0 -f mp4 -4 --width 1280 --decomb=\"7:2:6:9:1:80\" --loose-anamorphic --modulus 2 -m -x b-adapt=2";
					break;
				case Quality.iPad:
					Command += "-preset=\"iPad\"";
					break;
				case Quality.iPhone:
					Command += "-preset=\"iPhone 4\"";
					break;
				default:
					Command += "-e x264  -q 20.0 -r 30 --pfr  -a 1,1 -E faac,copy:ac3 -B 160,160 -6 dpl2,auto -R Auto,Auto -D 0.0,0.0 -f mp4 -4 --decomb=\"7:2:6:9:1:80\" --loose-anamorphic --modulus 2 -m -x b-adapt=2";
					break;
			}

			return HandBrake.HandBrake.Run(Command);
		}
	}
}

