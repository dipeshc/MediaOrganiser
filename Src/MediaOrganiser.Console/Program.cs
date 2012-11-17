using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;

namespace MediaOrganiser.Console
{
	class Program
	{
		public static void Main(string[] args)
		{
			// Run the command for the console input
			ConsoleCommandDispatcher.DispatchCommand(ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program)), args, System.Console.Out);
		}
	}
}
