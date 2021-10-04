using System;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	/// <summary>
	/// UnitAction controlling how units attack stuff.
	/// </summary>
	[Serializable]
	public class UnitAttackAction : UnitAction
	{
		public UnitActionType ActionType => UnitActionType.Attack;

		public UnitAttackType AttackType;
		public float MinDamage;
		public float MaxDamage;
		public float AttackRange;

		[Tooltip("Inverse of the attack duration in seconds")]
		public float AttackSpeed;

		[Tooltip("At which frame (% of total duration) does the unit land the strike?")]
		public float AttackUpswing;

		[Tooltip("Offset from view pivot to the spawn position of the projectile.")]
		public Vector3 ProjectileOffset;
		public float ProjectileVelocity;

		/// <summary>
		/// Ticks unit's action context.
		/// </summary>
		public UnitActionType Tick(Unit unit, ref UnitActionContext actionContext, float dT)
		{
			// if not currently in attack animation - break attack loop if needed.
			if (!actionContext.Started && ShouldBreakAttack(unit, actionContext.Target))
			{
				if (!actionContext.Target.IsValid)
					return UnitActionType.EndCurrentAction;

				actionContext.ResetProgress();
				return unit.MovementAction.Tick(unit, ref actionContext, dT);
			}

			// update orientation in case we are not orientated properly
			unit.RotateTowardTarget(actionContext.Target.Position, dT);

			// update progress
			var oldProgress = actionContext.Progress;
			var newProgress = oldProgress + dT * AttackSpeed;

			// land strike if reached the right frame
			if (!actionContext.Executed && newProgress >= AttackUpswing)
			{
				actionContext.Executed = true;
				ExecuteAction(unit, actionContext.Target);
			}

			// update progress
			actionContext.Progress = newProgress;
			actionContext.Started = true;

			// make sure progress doesn't overflow
			if (newProgress > 1f)
			{
				actionContext.ResetProgress();
				actionContext.Progress = Mathf.Clamp01(newProgress - 1f);
			}

			return UnitActionType.Attack;
		}

		private bool ShouldBreakAttack(Unit unit, UnitTargetInfo target)
		{
			if (!target.IsValid)
				return true;

			if (!unit.IsWithinAttackRange(target))
				return true;

			if (AttackType == UnitAttackType.Ranged)
				if (!unit.HasLineOfSight(target.Position))
					return true;

			return false;
		}

		public void ExecuteAction(Unit unit, UnitTargetInfo targetInfo)
		{
			switch (AttackType)
			{
				case UnitAttackType.Melee:
					if (targetInfo.TargetUnit != null)
					{
						targetInfo.TargetUnit.DealDamage(unit.GetRandomDamage(), unit);
					}
					break;
				case UnitAttackType.Ranged:
					FireProjectileAt(unit, targetInfo);
					break;
			}
		}

		public Projectile FireProjectileAt(Unit unit, UnitTargetInfo targetInfo)
		{
			var fromPosition = unit.GetPosition3D() + Quaternion.Euler(0, unit.Orientation, 0) * ProjectileOffset;
			var projectileDirection = (targetInfo.GetCenterPosition3D() - fromPosition).normalized;
			return unit.GameWorld.SpawnProjectile(unit, fromPosition, projectileDirection * ProjectileVelocity,
				targetInfo.TargetUnit, unit.GetRandomDamage());
		}
	}

	public enum UnitAttackType
	{
		Melee = 0,
		Ranged = 1
	}
}