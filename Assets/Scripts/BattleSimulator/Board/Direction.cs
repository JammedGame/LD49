using Unity.Mathematics;

namespace Game.Simulation.Board
{
	/// <summary>
	/// Directions, ordered CCW, names follow AlphaNum9 keyboard convention.
	/// </summary>
	public enum Direction
	{
		Dir4 = 0,
		Dir8 = 1,
		Dir6 = 2,
		Dir2 = 3
	}

	/// <summary>
	/// Corner directions, ordered CCW, names follow AlphaNum9 keyboard convention.
	/// </summary>
	public enum Corner
	{
		Dir1 = 0,
		Dir3 = 1,
		Dir7 = 2,
		Dir9 = 3,
	}

	public enum TileType
	{
		Flat = 0,
		FlatOccupied = 1,

		LeftSlope = 2,
		LeftSlope2 = 3,

		RightSlope = 4,
		RightSlope2 = 5,

		BackSlope = 6,
		BackSlope2 = 7,

		ForwardSlope = 8,
		ForwardSlope2 = 9,
	}

	public static class DirectionUtil
	{
		public static Direction GetOpposite(this Direction direction)
		{
			switch (direction)
			{
				case Direction.Dir4: return Direction.Dir6;
				case Direction.Dir6: return Direction.Dir4;
				case Direction.Dir2: return Direction.Dir8;
				case Direction.Dir8: return Direction.Dir2;
				default:
					throw new System.Exception("Undefined direction!");
			}
		}

		public static float2 ToVector(this Direction direction, float length = 1f)
		{
			switch (direction)
			{
				case Direction.Dir4: return new float2(-length, 0);
				case Direction.Dir6: return new float2(length, 0);
				case Direction.Dir2: return new float2(0, -length);
				case Direction.Dir8: return new float2(0, length);
				default:
					throw new System.Exception("Undefined direction!");
			}
		}

		public static float2 ToVector(this Corner direction, float axis = 1f)
		{
			switch (direction)
			{
				case Corner.Dir1: return new float2(-axis, -axis);
				case Corner.Dir3: return new float2(axis, -axis);
				case Corner.Dir7: return new float2(-axis, axis);
				case Corner.Dir9: return new float2(axis, axis);
				default:
					throw new System.Exception("Undefined corner!");
			}
		}

		public static Corner HorizontalComplement(this Corner direction)
		{
			switch (direction)
			{
				case Corner.Dir1: return Corner.Dir3;
				case Corner.Dir3: return Corner.Dir1;
				case Corner.Dir7: return Corner.Dir9;
				case Corner.Dir9: return Corner.Dir7;
				default:
					throw new System.Exception("Undefined corner!");
			}
		}

		public static Corner VerticalComplement(this Corner direction)
		{
			switch (direction)
			{
				case Corner.Dir1: return Corner.Dir7;
				case Corner.Dir3: return Corner.Dir9;
				case Corner.Dir7: return Corner.Dir1;
				case Corner.Dir9: return Corner.Dir3;
				default:
					throw new System.Exception("Undefined corner!");
			}
		}

		public static Direction GetEdgeDirection(float2 a, float2 b, float2 c)
		{
			var isHorizontal = math.abs(a.x - b.x) > math.abs(a.y - b.y);
			if (isHorizontal)
			{
				return a.y > c.y ? Direction.Dir8 : Direction.Dir2;
			}
			else
			{
				return a.x > c.x ? Direction.Dir6 : Direction.Dir4;
			}
		}
	}
}