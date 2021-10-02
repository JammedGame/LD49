using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    /// <summary>
    /// Implementation of either<Unit|Position>.
    /// </summary>
    public struct UnitTargetInfo
    {
        BattleObject targetObject;
        float2 targetPosition;

        public float2 Position => targetObject != null ? targetObject.GetPosition2D() : targetPosition;
        public float Radius => targetObject is Unit unit ? unit.Radius : 0f;
        public BattleObject TargetObject => targetObject;
        public Unit TargetUnit => targetObject as Unit;
		public bool IsValid => targetObject == null || targetObject.IsActive;

		public UnitTargetInfo(BattleObject targetObject)
        {
            this.targetObject = targetObject;
            this.targetPosition = targetObject?.GetPosition2D() ?? default;
        }

        public UnitTargetInfo(float2 targetPosition)
        {
            this.targetObject = null;
            this.targetPosition = targetPosition;
        }

        public static implicit operator UnitTargetInfo(BattleObject targetObject) => new UnitTargetInfo(targetObject);
        public static implicit operator UnitTargetInfo(float2 targetPosition) => new UnitTargetInfo(targetPosition);
    }
}