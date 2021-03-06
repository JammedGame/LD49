using System;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation.Board
{
	[Serializable]
	public class BoardData
	{
		[SerializeField] private int width;
		[SerializeField] private int height;
		[SerializeField] private Polygon[] polygons;

		int version; // if board gets changed, this will make other systems now they are out of sync with board state.

		public int Width => width;
		public int Height => height;
		public float MinX => 0;
		public float MaxX => width;
		public float MinY => 0;
		public float MaxY => height;
		public int Version => version;
		public Polygon[] Polygons => polygons;

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPoint(float2 point)
		{
			if (point.x < MinX) point.x = MinX;
			if (point.x > MaxX) point.x = MaxX;
			if (point.y < MinY) point.y = MinY;
			if (point.y > MaxY) point.y = MaxY;
			return point;
		}

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPosition(float2 point, float size)
		{
			if (point.x < MinX + size) point.x = MinX + size;
			if (point.x > MaxX - size) point.x = MaxX - size;
			if (point.y < MinY + size) point.y = MinY + size;
			if (point.y > MaxY - size) point.y = MaxY - size;
			return point;
		}

		/// <summary>
		/// Clamps given point to board borders.
		/// </summary>
		public float2 ClampPoint(float2 point, float2 size)
		{
			if (point.x < MinX + size.x) point.x = MinX + size.x;
			if (point.x > MaxX - size.x) point.x = MaxX - size.x;
			if (point.y < MinY + size.y) point.y = MinY + size.y;
			if (point.y > MaxY - size.y) point.y = MaxY - size.y;
			return point;
		}

		public Vector3 GetPosition3D(float2 pos)
		{
			return new Vector3(pos.x, 0, pos.y);
		}
	}

	[System.Serializable]
	public class Polygon
	{
		public float2[] Points;

		public bool IsPointInside(float2 point)
		{
			return Dist2Point(point) <= 0;
		}

		public float Dist2Point(float2 point)
		{
			// Compute circle position in the frame of the polygon.

			// Find the min separating edge.
			float separation = float.MinValue;
			int vertexCount = Points.Length;
			for (int i = 0; i < vertexCount; ++i)
			{
				var v0 = Points[i];
				var v1 = Points[(i + 1) % vertexCount];
				var v2 = Points[(i + 2) % vertexCount];

				// find the right normal.
				var edge = v1 - v0;
				var normal = math.normalize(new float2(edge.y, -edge.x));
				if (math.dot(v2 - v1, normal) > 0)
					normal = -normal;

				// find separation for this edge.
				float s = math.dot(normal, point - Points[i]);
				if (s > separation)
				{
					separation = s;
				}
			}

			return separation;
		}
	}
}