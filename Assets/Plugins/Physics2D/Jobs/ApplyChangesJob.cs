using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Physics2D
{
	public struct PositionVelocityChange
	{
		public int2 contact;
		public float2 leftPosOffset;
		public float2 leftVelocityOffset;
		public float2 rightPosOffset;
		public float2 rightVelocityOffset;

		public override string ToString()
		{
			return $"{contact} {leftPosOffset}-{leftVelocityOffset} {rightPosOffset}-{rightVelocityOffset}";
		}
	}

	[BurstCompile]
	public struct ApplyChangesJob : IJob
	{
		[ReadOnly] public NativeArray<PositionVelocityChange> positionVelocityChanges;

		public NativeArray<Transform> transforms;
		public NativeArray<float2> velocities;

		public void Execute()
		{
			for (int i = 0; i < positionVelocityChanges.Length; i++)
			{
				var change = positionVelocityChanges[i];
				if (all(change.contact == 0))
					continue;

				transforms[change.contact.x] = transforms[change.contact.x].Translate(change.leftPosOffset);
				velocities[change.contact.x] += change.leftVelocityOffset;

				transforms[change.contact.y] = transforms[change.contact.y].Translate(change.rightPosOffset);
				velocities[change.contact.y] += change.rightVelocityOffset;
			}
		}
	}
}