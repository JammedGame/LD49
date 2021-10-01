using System;
using Unity.Mathematics;

namespace Physics2D
{
	public struct Collider
	{
		public ColliderType Type; // type of the collider.
		public int BodyId; // id of the rigid body this collider is attached to (defines transform/mass/velocity/etc). Multiple colliders may be attached to the same body.
		public int DataStartIndex; // index of start of the collider's data
		public int DataLength; // amount of this colliders' data points
	}

	public enum ColliderType
	{
		Undefined = 0x0,
		Circle = 0x1,
		Edge = 0x2,
		Polygon = 0x3,
	}

	public enum ContactType
	{
		Circle2Circle = 0x11,
		Circle2Edge = 0x12,
		Circle2Polygon = 0x13,

		Edge2Circle = 0x21,
		Edge2Edge = 0x22,
		Edge2Polygon = 0x23,

		Polygon2Circle = 0x31,
		Polygon2Edge = 0x32,
		Polygon2Polygon = 0x33,
	}

	public static class ColliderTypeExtensions
	{
		public static ContactType GetContactTypeTo(this ColliderType left, ColliderType right)
		{
			return (ContactType)(((byte)left << 4) + (byte)right);
		}
	}
}