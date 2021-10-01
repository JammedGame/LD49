using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	public static partial class CollisionUtil
	{
		public static Manifold Circle2Edge(float2 circlePosition, float circleRadius, float2 edgeStart, float2 edgeEnd)
		{
			// line end to line start
			var edgeDelta = edgeEnd - edgeStart;

			// circle to line start
			var circle2LineRelative = circlePosition - edgeStart;

			// d_dist normalized
			var edgeLength = length(edgeDelta);
			var edgeNormal = edgeDelta / edgeLength;

			// project circle to start on a line
			var edgeProjectionLength = dot(circle2LineRelative, edgeNormal);
			if (edgeProjectionLength < 0f)
			{
				// circle is hitting edge.point0!
				var circle2edgeStartSq = lengthsq(circle2LineRelative);
				if (circle2edgeStartSq > circleRadius * circleRadius)
					return default;

				var circle2edgeStartDist = sqrt(circle2edgeStartSq);

				return new Manifold()
				{
					displacement = circleRadius - circle2edgeStartDist,
					normal = circle2LineRelative / circle2edgeStartDist
				};
			}
			else if (edgeProjectionLength > edgeLength)
			{
				// circle is hitting edge.point1!
				var circle2edgeEndSq = distancesq(circlePosition, edgeEnd);
				if (circle2edgeEndSq > circleRadius * circleRadius || circle2edgeEndSq < 1e-6)
					return default;

				var circle2edgeStartDist = sqrt(circle2edgeEndSq);
				var circle2LineEndRelative = circlePosition - edgeEnd;

				return new Manifold()
				{
					displacement = circleRadius - circle2edgeStartDist,
					normal = circle2LineEndRelative / circle2edgeStartDist
				};
			}
			else
			{
				// circle is hitting edge somewhere in between!
				var circle2edgeSq = lengthsq(circle2LineRelative) - edgeProjectionLength * edgeProjectionLength;
				if (circle2edgeSq > circleRadius * circleRadius || circle2edgeSq < 1e-6)
					return default;

				var circle2EdgeDist = sqrt(circle2edgeSq);
				var circle2LineIntersection = circle2LineRelative - edgeProjectionLength * edgeNormal;

				return new Manifold()
				{
					displacement = circleRadius - circle2EdgeDist,
					normal = circle2LineIntersection / circle2EdgeDist
				};
			}
		}
	}
}