using System;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float2 = Unity.Mathematics.float2;
using float2x2 = Unity.Mathematics.float2x2;

namespace Physics2D
{
    public readonly struct Transform
    {
        public readonly float2 Position;
        public readonly float2x2 Rotation;

		/// <summary>
		/// Identity transform.
		/// </summary>
		public static readonly Transform Identity = new Transform(float2.zero, 0f);

		public Transform(float2 position, float angleDegrees = 0f)
        {
			Position = position;

			float angleRadians = angleDegrees * Mathf.Deg2Rad;
			float c = math.cos(angleRadians);
			float s = math.sin(angleRadians);

			Rotation = new float2x2
            (
				c, -s,
				s, c
            );
		}

		public Transform(float2 position, float2x2 rotation)
		{
			Position = position;
			Rotation = rotation;
		}

		public Transform Translate(float2 offset) => new Transform(Position + offset, Rotation);

		/// <summary>
		/// Returns angle represented by this transform.
		/// </summary>
		public float GetAngle() => math.atan2(Rotation.c0.x, Rotation.c0.y);

        /// <summary>
		/// Converts point from local to world coordinates.
		/// </summary>
		public float2 TransformPoint(float2 point)
		{
			return math.mul(Rotation, point) + Position;
		}

		/// <summary>
		/// Transforms direction from local space to world space.
		/// </summary>
		public float2 TransformDirection(float2 normal)
		{
			return mul(Rotation, normal);
		}

		/// <summary>
		/// Convers point from world to local coordinates.
		/// </summary>
		public float2 InverseTransformPoint(float2 point)
		{
			return mul(inverse(Rotation), point - Position);
		}
	}
}
