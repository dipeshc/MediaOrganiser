using System.Collections.Generic;
using System.IO;

namespace System.Logger
{
	public class Logger
	{
		public TextWriter StdOut = Console.Out;
		public TextWriter StdErr = Console.Error;

		private static Dictionary<string, Logger> logs = new Dictionary<string, Logger>();
		public static Logger Log(string logName = "Global")
		{
			if(!logs.ContainsKey(logName))
			{
				logs[logName] = new Logger();
			}
			return logs[logName];
		}
	}
}