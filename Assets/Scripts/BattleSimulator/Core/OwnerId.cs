using System;

namespace Game.Simulation
{
	public enum OwnerId
	{
		Player1 = 0,
		Player2 = 1,
	}

	public static class OwnerExtensions
	{
		public static OwnerId GetOpposite(this OwnerId owner)
		{
			switch (owner)
			{
				case OwnerId.Player1:
					return OwnerId.Player2;
				case OwnerId.Player2:
					return OwnerId.Player1;
				default:
					throw new Exception();
			}
		}
	}
}