using System;
using System.Files;
using HandBrake;

namespace MediaOrganiser.Convertor
{
	public static class Convertor
	{
		public static String ConvertForiPad(IFile InputFile, IFile OutputFile)
		{
			return HandBrake.HandBrake.Run(String.Format(@"-i '{0}' -o '{1}' --preset=iPad --subtitle-burn", InputFile.FullName, OutputFile.FullName));
		}
	}
}

