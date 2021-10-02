using Unity.Mathematics;
using UnityEngine;

public static class Math2D
{
	public static bool LineIntersecsCircle(float from_x, float from_y, float to_x, float to_y, float circle_x, float circle_y, float radius)
	{
		var d_x = to_x - from_x;
		var d_y = to_y - from_y;

		var f_x = from_x - circle_x;
		var f_y = from_y - circle_y;

		// forms quadratic eq, checks if results exist_
		var a = d_x * d_x + d_y * d_y;
		var b = 2 * (f_x * d_x + f_y * d_y);
		var c = f_x * f_x + f_y * f_y - radius * radius;
		var det = b * b - 4 * a * c;

		return det >= 0;
	}

	public static bool PathObstructed(float from_x, float from_y, float to_x, float to_y, float circle_x, float circle_y, float radius, float my_radius)
	{
		var f_x = from_x - circle_x;
		var f_y = from_y - circle_y;
		var f_sqr = f_x * f_x + f_y * f_y;

		var d_x = to_x - from_x;
		var d_y = to_y - from_y;
		var d_sqr = d_x * d_x + d_y * d_y;
		if (d_sqr <= f_sqr) { return false; }

		// forms quadratic eq, checks if results exist_
		var r = radius + my_radius;
		var a = d_sqr;
		var b = 2 * (f_x * d_x + f_y * d_y);
		var c = f_sqr - r * r;
		var det = b * b - 4 * a * c;

		return det >= 0f && b < 0f;
	}

	public static float LineDistance(float from_x, float from_y, float to_x, float to_y, float circle_x, float circle_y)
	{
		var d_x = to_x - from_x;
		var d_y = to_y - from_y;
		var d_sqr = d_x * d_x + d_y * d_y;

		return Mathf.Abs(d_y * circle_x - d_x * circle_y + to_x * from_y - to_y * from_x)
			/ Mathf.Sqrt(d_sqr);
	}

	public static float GetDistanceToPoint(float2 pointA, float2 pointB, float2 point)
	{
		float lineDistanceSQ = math.distancesq(pointB, pointA);
		if (lineDistanceSQ < 0.000001f)
			return 0.0f;

		float u = ((point.x - pointA.x) * (pointB.x - pointA.x) + (point.y - pointA.y) * (pointB.y - pointA.y)) / lineDistanceSQ;
		u = Mathf.Clamp01(u);
		var intersect = new float2();
		intersect.x = pointA.x + u * (pointB.x - pointA.x);
		intersect.y = pointA.y + u * (pointB.y - pointA.y);
		return math.distance(intersect, point);
	}

	public static float LineDistance(float2 from, float2 to, float2 circle)
	{
		return LineDistance(from.x, from.y, to.x, to.y, circle.x, circle.y);
	}
}