using System;
using Unity.Mathematics;

namespace Game.Simulation
{
	public enum SpawnPosition
	{
		Custom,
		Left,
		Center,
		Right
	}

	public static class SpawnPositionExtensions
	{
		private static readonly float2 SpawnPositionLeft = new float2(40, 10);
		private static readonly float2 SpawnPositionCenter = new float2(75, 50);
		private static readonly float2 SpawnPositionRight = new float2(40, 90);

		public static float2 ToFloat2(this SpawnPosition spawnPosition)
		{
			switch (spawnPosition)
			{
				case SpawnPosition.Custom:
					return float2.zero;
				case SpawnPosition.Left:
					return SpawnPositionLeft;
				case SpawnPosition.Center:
					return SpawnPositionCenter;
				case SpawnPosition.Right:
					return SpawnPositionRight;
				default:
					throw new ArgumentOutOfRangeException(nameof(spawnPosition), spawnPosition, null);
			}
		}
	}
}