using System;

namespace System.Threading.Helpers
{
	public class Helper
	{
		public static Int32 WaitTime = 2500;
		
		public static void RunWhenThreadAvailable(LockableInt ThreadsAvailable, Int32 ThreadsRequired, Func<Void> Action)
		{
			// Setup while loop todo waiting.
			while(true)
			{
				// Lock ThreadsAvailable todo check if can continue.
				lock(ThreadsAvailable)
				{
					if(ThreadsAvailable.Value >= ThreadsRequired)
					{
						// Allocate space away from ThreadsAvailable.
						ThreadsAvailable.Value -= ThreadsRequired;
						break;
					}
				}
				
				// Wait.
				Thread.Sleep(WaitTime);
			}
			
			// Do action.
			try
			{
				Action();
			}
			finally
			{
				// Allocate space back to ThreadsAvailable.
				lock(ThreadsAvailable)
				{
					ThreadsAvailable.Value += ThreadsRequired;
				}
			}
		}
	}

}

