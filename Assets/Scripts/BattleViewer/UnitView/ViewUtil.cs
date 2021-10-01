using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace Game.View
{
	public static class ViewUtil
    {
        public static Vector3 ConvertTo3D(float2 data, float y = 0f)
        {
            return new Vector3(data.x, y, data.y);
        }

        public static float2 ConvertTo2D(Vector3 data)
        {
            return new float2(data.x, data.z);
        }

        public static Quaternion GetRotation3D(this Unit data)
        {
            return Quaternion.Euler(0, data.Orientation, 0);
        }
    }
}