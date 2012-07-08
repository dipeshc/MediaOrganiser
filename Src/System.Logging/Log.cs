using System;
using System.IO;

namespace System.Logging
{
	public static class Log
	{
		//public static TextWriter OutputStream = Console.Out;

		public static void WriteLine(String Format, params Object[] Objects)
		{
			WriteLine(String.Format(Format, Objects));
		}

		public static void WriteLine(String Line)
		{
			String LogLine = String.Format("[{0}] {1}\n", DateTime.Now, Line);
			Console.WriteLine(LogLine);
			//OutputStream.WriteLine(LogLine);
		}
	}
}

