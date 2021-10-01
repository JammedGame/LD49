using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	[BurstCompile]
	public struct UpdateVelocitiesJob : IJobParallelFor
	{
		public float dT;
		public float VelocityDragConstant;
		public float VelocityDragRelative;

		public NativeArray<Transform> transforms;
		public NativeArray<float2> velocities;

		public void Execute(int i)
		{
			// update position by velocity
			transforms[i] = transforms[i].Translate(velocities[i] * dT);

			// slow down all velocities by global drag values
			var velocityMagnitude = length(velocities[i]);
			if (velocityMagnitude > 0)
			{
				var newVelocityMagnitude = (velocityMagnitude - VelocityDragConstant) * VelocityDragRelative;
				if (newVelocityMagnitude <= 0)
				{
					velocities[i] = 0;
				}
				else
				{
					velocities[i] *= newVelocityMagnitude / velocityMagnitude;
				}
			}
		}
	}
}