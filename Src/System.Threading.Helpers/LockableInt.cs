using System;

namespace System.Threading.Helpers
{
	public class LockableInt
	{
		public Int32 Value;
		
		public LockableInt (Int32 Value)
		{
			this.Value = Value;
		}
	}
}

