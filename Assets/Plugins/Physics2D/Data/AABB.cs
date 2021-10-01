using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	public struct AABB
	{
		public float2 min;
		public float2 max;

		public AABB(float2 min, float2 max)
		{
			this.min = min;
			this.max = max;
		}

		public static bool TestIntersection(AABB left, AABB right)
		{
			var d = new float4(right.min - left.max, left.min - right.max);
			return all(d < 0);
		}

		public bool Contains(float2 p)
		{
			return p.x >= min.x && p.y >= min.y
				&& p.x <= max.x && p.y <= max.y;
		}
	}
}