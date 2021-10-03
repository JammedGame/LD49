using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    /// <summary>
    /// Implementation of either<Unit|Position>.
    /// </summary>
    public struct UnitTargetInfo
    {
        Unit targetUnit;
        float2 targetPosition;

        public float2 Position => targetUnit != null ? targetUnit.GetPosition2D() : targetPosition;
        public float Radius => targetUnit is Unit unit ? unit.Radius : 0f;
        public Unit TargetUnit => targetUnit;
		public bool IsValid => targetUnit == null || targetUnit.IsActive;

		public UnitTargetInfo(Unit targetObject)
        {
            this.targetUnit = targetObject;
            this.targetPosition = default;
        }

        public UnitTargetInfo(float2 targetPosition)
        {
            this.targetUnit = null;
            this.targetPosition = targetPosition;
        }

        public bool Equals(UnitTargetInfo rhs)
        {
			return rhs.targetUnit == targetUnit
				&& rhs.targetPosition.Equals(targetPosition);
		}

        public static implicit operator UnitTargetInfo(Unit targetObject) => new UnitTargetInfo(targetObject);
        public static implicit operator UnitTargetInfo(float2 targetPosition) => new UnitTargetInfo(targetPosition);

        public override string ToString()
        {
			return targetUnit != null ? targetUnit.ToString() : targetPosition.ToString();
        }
    }
}