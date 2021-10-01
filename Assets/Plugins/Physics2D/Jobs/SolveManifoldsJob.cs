using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	[BurstCompile]
	internal struct SolveManifoldsJob : IJobParallelFor
	{
		public float BounceAbsorbConstant;
		public float BounceAbsorbRelative;

		[ReadOnly] public NativeArray<Collider> colliders;
		[ReadOnly] public NativeArray<int2> contacts;
		[ReadOnly] public NativeArray<float> masses;
		[ReadOnly] public NativeArray<float2> velocities;
		[ReadOnly] public NativeArray<Manifold> manifolds;
		[ReadOnly] public NativeArray<bool> isStatic;

		[WriteOnly] public NativeArray<PositionVelocityChange> positionVelocityChanges;

		public void Execute(int i)
		{
			var displacement = manifolds[i].displacement;
			if (displacement < 0.001f)
				return;

			var normal = manifolds[i].normal;
			var change = new PositionVelocityChange();
			var contact = contacts[i];

			var leftCollider = colliders[contact.x];
			var rightCollider = colliders[contact.y];

			var leftBodyId = leftCollider.BodyId;
			var rightBodyId = rightCollider.BodyId;

			var leftIsStatic = isStatic[leftBodyId];
			var rightIsStatic = isStatic[rightBodyId];
			if (leftIsStatic && rightIsStatic)
				return;

			var leftMass = masses[leftBodyId];
			var rightMass = masses[rightBodyId];

			var leftVelocity = velocities[leftBodyId];
			var rightVelocity = velocities[rightBodyId];

			// update position based on the displacement needed
			if (leftIsStatic)
			{
				change.rightPosOffset -= normal * displacement;

				// redistribute velocities.
				var v2 = max(0, dot(rightVelocity, normal) - BounceAbsorbConstant);
				change.rightVelocityOffset -= normal * 2 * v2;
			}
			else if (rightIsStatic)
			{
				change.leftPosOffset += normal * displacement;

				// redistribute velocities.
				var v1 = max(0, dot(leftVelocity, -normal) - BounceAbsorbConstant);
				change.leftVelocityOffset += normal * 2 * v1;
			}
			else
			{
				// distribute displacement based on the mass ratio.
				var totalForce = normal * displacement;
				var totalMass = leftMass + rightMass;
				change.leftPosOffset += totalForce * (totalMass > 0 ? rightMass / totalMass : 0.5f);
				change.rightPosOffset -= totalForce * (totalMass > 0 ? leftMass / totalMass : 0.5f);

				// redistribute velocities.
				var v1 = max(0, dot(leftVelocity, -normal) - BounceAbsorbConstant);
				var v2 = max(0, dot(rightVelocity, normal) - BounceAbsorbConstant);
				var f1 = min(v1 * rightMass, 2 * v1 * leftMass);
				var f2 = min(v2 * leftMass, 2 * v2 * rightMass);
				var redistribution = normal * (f1 + f2);

				var initialKinetic = leftMass * lengthsq(leftVelocity) + rightMass * lengthsq(rightVelocity) * BounceAbsorbRelative;
				redistribution = CheckKineticEnergy(redistribution, initialKinetic, leftVelocity, rightVelocity, leftMass, rightMass);

				change.leftVelocityOffset += redistribution / leftMass;
				change.rightVelocityOffset -= redistribution / rightMass;
			}

			// write down.
			change.contact = new int2(leftBodyId, rightBodyId);
			positionVelocityChanges[i] = change;
		}

		private float2 CheckKineticEnergy(float2 redistribution, float initialKinetic, float2 leftVel, float2 rightVel, float leftMass, float rightMass)
		{
			var bestRedistribution = redistribution;
			var bestError = float.MaxValue;

			for (float x = 1f; x >= 0.5f; x -= 0.01f)
			{
				var testRedistribution = redistribution * x;
				var testLeftVel = leftVel + testRedistribution / leftMass;
				var testRightVel = rightVel - testRedistribution / rightMass;
				var testKinetic = leftMass * lengthsq(testLeftVel) + rightMass * lengthsq(testRightVel);
				var error = abs(testKinetic - initialKinetic);
				if (error < bestError)
				{
					bestError = error;
					bestRedistribution = testRedistribution;
				}
			}

			return bestRedistribution;
		}
	}
}