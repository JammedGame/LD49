using System;
using Game.Simulation;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace Game.View
{
	public class UnitView : BattleObjectView
	{
		[Header("Animation")]
		public AnimationInfo MovementAnimation;
		public AnimationInfo IdleAnimation;
		public AnimationInfo PrimaryAttackAnimation;

		Animation animationComponent;
		AnimationInfo currentClip;

		public override void OnInitialized(BattleObject data)
		{
			if (!(data is Unit))
			{
				Debug.LogError($"{this} only accepts data of type Unit, not {data}", this);
				return;
			}

			animationComponent = GetComponentInChildren<Animation>();
		}

		public override void SyncView(float dT)
		{
			var unit = (Unit)Data;

			// update transform
			SyncUnitTransform(unit);
			SyncUnitAnimation(unit);
		}

		private void SyncUnitTransform(Unit unit)
		{
			if (any(isnan(unit.Position)))
			{
				Debug.LogError($"Unit {unit} postition is nan!");
				return;
			}

			transform.SetPositionAndRotation
			(
				unit.GetPosition3D(),
				unit.GetRotation3D()
			);
		}

		private void SyncUnitAnimation(Unit unit)
		{
			// get target clip unit should play right now.
			AnimationInfo targetClip = GetTargetClip(unit);
			var IsAnimationControlledBySimulation = unit.CurrentActionType.IsAnimationControlledBySimulation();

			// clip change logic if needed.
			if (targetClip != currentClip)
			{
				currentClip = targetClip;
				animationComponent.CrossFade(currentClip.ClipName, targetClip.Crossfade, PlayMode.StopAll);

				AnimationState state = animationComponent[currentClip.ClipName];
				state.speed = IsAnimationControlledBySimulation ? 0 : currentClip.Speed;
			}

			// update animation progress manually if needed.
			if (IsAnimationControlledBySimulation)
			{
				AnimationState state = animationComponent[currentClip.ClipName];
				state.normalizedTime = unit.CurrentActionProgress;
			}
		}

		/// <summary>
		/// Returns animation clip name which should be played next.
		/// </summary>
		public virtual AnimationInfo GetTargetClip(Unit unit)
		{
			switch(unit.CurrentActionType)
			{
				case UnitActionType.Attack:
					return PrimaryAttackAnimation;

				case UnitActionType.Movement:
					return MovementAnimation;

				default:
					return IdleAnimation;
			}
		}
	}

	[Serializable]
	public class AnimationInfo
	{
		public string ClipName = "Idle";
		public float Speed = 1f;
		public float Crossfade = 0.15f;
	}
}