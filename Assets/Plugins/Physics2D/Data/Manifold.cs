using Unity.Mathematics;

namespace Physics2D
{
	public struct Manifold
	{
		public float2 normal;
		public float displacement;

		public Manifold Invert() => new Manifold()
		{
			normal = -normal,
			displacement = displacement
		};

		public float2 offset => normal * displacement;
	}
}