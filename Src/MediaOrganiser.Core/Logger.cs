using System.Collections.Generic;
using System.IO;

namespace System.Logger
{
	public class Logger
	{
		public TextWriter StdOut = Console.Out;
		public TextWriter StdErr = Console.Error;

		private static Dictionary<string, Logger> Logs = new Dictionary<string, Logger>();
		public static Logger Log(string LogName = "Global")
		{
			if(!Logs.ContainsKey(LogName))
			{
				Logs[LogName] = new Logger();
			}
			return Logs[LogName];
		}
	}
}