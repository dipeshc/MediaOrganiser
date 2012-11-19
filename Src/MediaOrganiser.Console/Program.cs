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
			//ConsoleCommandDispatcher.DispatchCommand(ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program)), args, System.Console.Out);

			var aDate = new DateTime(2000, 1, 1, 0, 0 , 0, DateTimeKind.Unspecified);
			var response = MediaOrganiser.Core.TagModifier.SetTVShowDetails("/Users/Dipesh/Desktop/Test/1.mp4", "TVShow1", 10, 5, "S10E05", aDate, "aDescription", "dipeshTV", null);
			System.Console.WriteLine(response);
		}
	}
}
