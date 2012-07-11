using System;
using System.Files;
using HandBrake;

namespace MediaOrganiser.Convertor
{
	public static class Convertor
	{
		public static String ConvertForiPad(IFile InputFile, IFile OutputFile)
		{
			String EscapedInputFullName = InputFile.FullName.Replace("\\", "\\\\").Replace("\"", "\\\"");
			return HandBrake.HandBrake.Run(String.Format("-i \"{0}\" -o \"{1}\" --preset=iPad --subtitle-burn", EscapedInputFullName, OutputFile.FullName));
		}
	}
}

