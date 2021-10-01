using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	[BurstCompile]
	public struct CalculateAABBJob : IJobParallelFor
	{
		[ReadOnly] public NativeArray<Collider> colliders;
		[ReadOnly] public NativeArray<Transform> transforms;
		[ReadOnly] public NativeArray<float2> colliderData;

		[WriteOnly] public NativeArray<AABB> aabbBuffer;

		public void Execute(int i)
		{
			aabbBuffer[i] = GetAABB(i);
		}

		public AABB GetAABB(int i)
		{
			switch (colliders[i].Type)
			{
				case ColliderType.Circle: return GetCircleAABB(i);
				case ColliderType.Edge: return GetEdgeAABB(i);
				case ColliderType.Polygon: return GetPolygonAABB(i);
				default: return default;
			}
		}

		private AABB GetPolygonAABB(int i)
		{
			var aabb = new AABB(float.MaxValue, float.MinValue);
			var parentId = colliders[i].BodyId;
			var transform = transforms[parentId];
			var dataStart = colliders[i].DataStartIndex;
			var dataEnd = dataStart + colliders[i].DataLength;

			for (int p = dataStart; p < dataEnd; p++)
			{
				var point = transform.TransformPoint(colliderData[p]);
				aabb.min = min(aabb.min, point);
				aabb.max = max(aabb.max, point);
			}

			return aabb;
		}

		private AABB GetEdgeAABB(int i)
		{
			var parentId = colliders[i].BodyId;
			var dataStart = colliders[i].DataStartIndex;
			var transform = transforms[parentId];
			var point1 = transform.TransformPoint(colliderData[dataStart]);
			var point2 = transform.TransformPoint(colliderData[dataStart + 1]);
			return new AABB(min(point1, point2), max(point2, point1));
		}

		private AABB GetCircleAABB(int i)
		{
			var parentId = colliders[i].BodyId;
			var transform = transforms[parentId];
			var dataStart = colliders[i].DataStartIndex;
			var circlePosition = transform.TransformPoint(colliderData[dataStart]);
			var radius = colliderData[dataStart + 1];
			return new AABB(circlePosition - radius, circlePosition + radius);
		}
	}
}