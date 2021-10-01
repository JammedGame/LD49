using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	public static partial class CollisionUtil
	{
		public static Manifold Circle2Circle(float2 position1, float radius1, float2 position2, float radius2)
		{
			var delta = position1 - position2;
			var distSq = lengthsq(delta);

			var totalSize = radius1 + radius2;
			if (totalSize * totalSize < distSq)
				return default;

			var dist = sqrt(distSq);
			if (dist <= 0f)
				return default;

			return new Manifold()
			{
				displacement = totalSize - dist,
				normal = delta / dist
			};
		}

	}
}