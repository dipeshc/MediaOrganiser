using System;

namespace System.Logging
{
	public enum LogLocation
	{
		Console,
		File
	}

	public static class Log
	{
		public static Boolean LoggingOn = true;
		public static LogLocation LogLocation = LogLocation.Console;
		public static System.Files.IFile LogFile = new System.Files.File("app.log");

		public static void WriteLine(String Format, params Object[] Objects)
		{
			WriteLine(String.Format(Format, Objects));
		}

		public static void WriteLine(String Line)
		{
			if(!LoggingOn)
			{
				return;
			}

			String LogLine = String.Format("[{0}] {1}\n", DateTime.Now, Line);

			if(LogLocation == LogLocation.Console)
			{
				Console.Write(LogLine);
			}
			else if(LogLocation == LogLocation.File)
			{
				System.IO.StreamWriter StreamWriter = new System.IO.StreamWriter(LogFile.FullName);
				StreamWriter.WriteLine(LogLine);
				StreamWriter.Close();
			}
		}
	}
}

