using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	public static partial class CollisionUtil
	{
		public static Manifold Circle2Polygon(float2 circlePosition, float radius, in Transform polygonTransform, [ReadOnly] NativeSlice<float2> vertices)
		{
			// Compute circle position in the frame of the polygon.
			float2 circleLocal = polygonTransform.InverseTransformPoint(circlePosition);

			// Find the min separating edge.
			float separation = float.MinValue;
			int vertexCount = vertices.Length;
			float2 edgeStart = default, edgeEnd = default, edgeNormal = default;

			for (int i = 0; i < vertexCount; ++i)
			{
				var v0 = vertices[i];
				var v1 = vertices[(i + 1) % vertexCount];
				var v2 = vertices[(i + 2) % vertexCount];

				// find the right normal.
				var edge = v1 - v0;
				var normal = normalize(new float2(edge.y, -edge.x));
				if (dot(v2 - v1, normal) > 0)
					normal = -normal;

				// find separation for this edge.
				float s = dot(normal, circleLocal - vertices[i]);
				if (s > separation)
				{
					separation = s;
					edgeStart = v0;
					edgeEnd = v1;
					edgeNormal = normal;
				}
			}

			// circle is inside polygon!
			if (separation < 0)
			{
				return new Manifold()
				{
					displacement = radius - separation,
					normal = polygonTransform.TransformDirection(edgeNormal)
				};
			}

			// circle is too far away
			if (separation > radius)
				return default;

			// circle is inside range - check if we should use normal, or vertex.
			var manifold = Circle2Edge(circleLocal, radius, edgeStart, edgeEnd);
			manifold.normal = polygonTransform.TransformDirection(manifold.normal);
			return manifold;
		}
	}
}