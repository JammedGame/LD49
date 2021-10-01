using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Physics2D
{
	[BurstCompile]
	public struct BroadPhaseJob : IJob
	{
		[ReadOnly] public NativeArray<bool> isStatic;
		[ReadOnly] public NativeArray<Collider> colliders;
		[ReadOnly] public NativeArray<AABB> aabbBuffer;

		[WriteOnly] public NativeList<int2> contacts;

		public void Execute()
		{
			for (int i = 0; i < aabbBuffer.Length; i++)
			{
				for (int j = i + 1; j < aabbBuffer.Length; j++)
				{
					if (AABB.TestIntersection(aabbBuffer[i], aabbBuffer[j]))
					{
						var leftBodyId = colliders[i].BodyId;
						var rightBodyId = colliders[j].BodyId;
						if (leftBodyId == rightBodyId)
							continue;
						if (isStatic[leftBodyId] && isStatic[rightBodyId])
							continue;

						contacts.Add(new int2(i, j));
					}
				}
			}
		}
	}
}