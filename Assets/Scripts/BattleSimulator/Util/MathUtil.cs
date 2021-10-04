using Unity.Mathematics;
using UnityEngine;

public static class MathUtil
{
	public static float ConvertDirectionToOrientation(float2 direction)
	{
		if (direction.x == 0f && direction.y == 0f)
			return 0;

		return Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
	}

	public static float ConvertDirectionToOrientation(float2 from, float2 to)
	{
		if (to.Equals(from))
			return 0;

		return Mathf.Atan2(to.x - from.x, to.y - from.y) * Mathf.Rad2Deg;
	}

	public static float2 ConvertOrientationToDirection(float orientation)
	{
		var radians = !float.IsNaN(orientation) ? orientation * Mathf.Deg2Rad : 0;
		return new float2(math.sin(radians), math.cos(radians));
	}

	public static float3 ConvertOrientationToDirection3D(float orientation)
	{
		var radians = !float.IsNaN(orientation) ? orientation * Mathf.Deg2Rad : 0;
		return new float3(math.sin(radians), 0, math.cos(radians));
	}

	public static float3 GetDirection(float3 from, float3 to)
	{
		return math.normalizesafe(to - from);
	}

	public static float GetDistanceToLine(Vector3 p, Vector3 from, Vector3 to, float height, float radius)
	{
		p.y /= height;
		from.y /= height;
		to.y /= height;

		p.x /= radius;
		from.x /= radius;
		to.x /= radius;
		p.z /= radius;
		from.z /= radius;
		to.z /= radius;

		Vector3 d = (to - from).normalized;
		Vector3 v = p - from;
		float t = Vector3.Dot(v, d);
		Vector3 P = from + t * d;
		return Vector3.Distance(P, p);
	}
}