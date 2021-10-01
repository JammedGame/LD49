using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Physics2D
{
	[BurstCompile]
	public struct NarrowPhaseJob : IJobParallelFor
	{
		[ReadOnly] public NativeArray<int2> contacts;
		[ReadOnly] public NativeArray<Transform> transforms;
		[ReadOnly] public NativeArray<Collider> colliders;
		[ReadOnly] public NativeArray<float2> colliderData;

		[WriteOnly] public NativeArray<Manifold> manifolds;

		public void Execute(int index)
		{
			manifolds[index] = GetManifoldForIndex(index);
		}

		private Manifold GetManifoldForIndex(int index)
		{
			var contact = contacts[index];

			var colliderLeft = colliders[contact.x];
			var colliderRight = colliders[contact.y];
			if (colliderLeft.BodyId == colliderRight.BodyId)
				throw new Exception();

			var transformLeft = transforms[colliderLeft.BodyId];
			var transformRight = transforms[colliderRight.BodyId];
			var leftColliderData = colliderData.Slice(colliderLeft.DataStartIndex, colliderLeft.DataLength);
			var rightColliderData = colliderData.Slice(colliderRight.DataStartIndex, colliderRight.DataLength);

			var contactType = colliderLeft.Type.GetContactTypeTo(colliderRight.Type);
			switch (contactType)
			{
				case ContactType.Circle2Circle:
				{
					var circle1Position = transformLeft.TransformPoint(leftColliderData[0]);
					var circle1Radius = leftColliderData[1].x;
					var circle2Position = transformRight.TransformPoint(rightColliderData[0]);
					var circle2Radius = rightColliderData[1].x;
					return CollisionUtil.Circle2Circle(circle1Position, circle1Radius, circle2Position, circle2Radius);
				}

				case ContactType.Circle2Edge:
				{
					var circlePosition = transformLeft.TransformPoint(leftColliderData[0]);
					var circleRadius = leftColliderData[1].x;
					var edgeStart = transformRight.TransformPoint(rightColliderData[0]);
					var edgeEnd = transformRight.TransformPoint(rightColliderData[1]);
					return CollisionUtil.Circle2Edge(circlePosition, circleRadius, edgeStart, edgeEnd);
				}

				case ContactType.Edge2Circle:
				{
					var circlePosition = transformRight.TransformPoint(rightColliderData[0]);
					var circleRadius = rightColliderData[1].x;
					var edgeStart = transformLeft.TransformPoint(leftColliderData[0]);
					var edgeEnd = transformLeft.TransformPoint(leftColliderData[1]);
					return CollisionUtil.Circle2Edge(circlePosition, circleRadius, edgeStart, edgeEnd).Invert();
				}

				case ContactType.Polygon2Circle:
				{
					var circlePosition = transformRight.TransformPoint(rightColliderData[0]);
					var circleRadius = rightColliderData[1].x;
					return CollisionUtil.Circle2Polygon(circlePosition, circleRadius, transformLeft, leftColliderData).Invert();
				}

				case ContactType.Circle2Polygon:
				{
					var circlePosition = transformLeft.TransformPoint(leftColliderData[0]);
					var circleRadius = leftColliderData[1].x;
					return CollisionUtil.Circle2Polygon(circlePosition, circleRadius, transformRight, rightColliderData);
				}

				case ContactType.Polygon2Edge:
				case ContactType.Edge2Polygon:
				case ContactType.Edge2Edge:
					var edge1Start = transformLeft.TransformPoint(leftColliderData[0]);
					var edge1End = transformLeft.TransformPoint(leftColliderData[1]);
					var edge2Start = transformRight.TransformPoint(rightColliderData[0]);
					var edge2End = transformRight.TransformPoint(rightColliderData[1]);
					return CollisionUtil.Edge2Edge(edge1Start, edge1End, edge2Start, edge2End);

				default:
					throw new Exception();
			}

		}
	}
}