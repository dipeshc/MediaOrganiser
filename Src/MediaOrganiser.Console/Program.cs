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
			// locate any commands in the assembly (or use an IoC container, or whatever source)
			var commands = GetCommands();
			
			// run the command for the console input
			ConsoleCommandDispatcher.DispatchCommand(commands, args, System.Console.Out);
		}
		
		static IEnumerable<ConsoleCommand> GetCommands()
		{
			return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
		}
	}
}
