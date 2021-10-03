using System;
using BattleSimulator.Brains;
using BattleSimulator.Spells;
using Game.Simulation.Board;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using float2 = Unity.Mathematics.float2;

namespace Game.Simulation
{
	public class Unit : BattleObject
	{
		// type information
		public readonly UnitSettings Settings;

		// action state machine
		private UnitAction currentAction;
		private UnitActionType currentActionType;
		private UnitActionContext actionContext;
		private float health;
		private bool isInvulnerable;

		// stats
		public Stat Speed;

		// transform state
		public float2 Position;
		public float2 Velocity;
		public float Orientation;

		// actions
		public readonly UnitMoveWithPathfinding MovementAction = new UnitMoveWithPathfinding();

		// API
		public float CurrentActionProgress => actionContext.Progress;
		public UnitActionType CurrentActionType => currentActionType;
		public UnitTargetInfo CurrentTarget => actionContext.Target;
		public UnitAction CurrentAction => currentAction;
		public float2 GetDirection() => MathUtil.ConvertOrientationToDirection(Orientation);
		public bool IsAttacking => currentActionType == UnitActionType.Attack;
		public bool IsMoving => currentActionType == UnitActionType.Movement;
		public float Radius => Settings.Size;
		public float Health => health;
		public bool IsInvulnerable => isInvulnerable;
		public float HealthPercent => Mathf.Clamp01(health / Settings.Health);
		public virtual bool IsStatic => false; // for collision purposes
		public bool IsValidAttackTarget => IsActive && !IsInvulnerable;

		public override string ViewPath => $"View/UnitViews/{Settings.name}View";

		private IBrain brain;

		public override Vector3 GetPosition3D() => GameWorld.Board.GetPosition3D(Position);
		public override float2 GetPosition2D() => Position;

		public Unit(GameWorld gameWorld, UnitSettings unitSettings, float2 position, OwnerId owner, BattleObject parent)
			: base(gameWorld, owner, parent)
		{
			currentAction = DoNothingAction.Instance;
			health = unitSettings.Health;
			Settings = unitSettings;
			Position = position;
			Speed = unitSettings.Speed;
			ResetModifiers();
		}

		public virtual void ResetModifiers()
		{
			isInvulnerable = Settings.IsInvulnerable;
			Speed.Reset();
		}

		public virtual void Tick()
		{
			// check what should be my current action.
			var decision = brain?.Think(this);
			if (decision != null)
				StartAction(decision.Action, decision.Target);

			// execute active action.
			currentActionType = currentAction.Tick(this, ref actionContext, GameTick.TickDuration);

			// check if action is finished.
			if (currentActionType == UnitActionType.EndCurrentAction)
			{
				OrderIdle();
			}
		}

		#region Unit orders

		public void OrderIdle()
		{
			StartAction(DoNothingAction.Instance);
		}

		public void OrderMoveInDirection(float2 direction)
		{
			StartAction(UnitMoveWithDirectionAction.Instance, direction);
		}

		/// <summary>
		/// Makes unit enter move to point action.
		/// </summary>
		public void OrderMoveToPoint(float2 point)
		{
			point = GameWorld.Board.ClampPosition(point, Radius);
			if (point.Equals(Position))
				return;

			StartAction(MovementAction, point);
		}

		public void OrderAttacking(UnitTargetInfo attackTarget)
		{
			if (!attackTarget.IsValid
			|| attackTarget.TargetUnit != null && !attackTarget.TargetUnit.IsValidAttackTarget)
			{
				Debug.LogWarning($"Skipped attacking {attackTarget}");
				return;
			}

			StartAction(Settings.PrimaryAttack, attackTarget);
		}

		public void OrderSpellCast(SpellSettings spellSettings, UnitTargetInfo targetInfo)
		{
			StartAction(new CastSpellAction(spellSettings, Settings.CastUpswing), targetInfo);
		}

		public void StartAction(UnitAction newAction, UnitTargetInfo target = default)
		{
			if (newAction == null)
				throw new NullReferenceException();

			// don't reset context if order is the same.
			if (this.currentAction == newAction && actionContext.Target.Equals(target))
			{
				return;
			}

			currentAction = newAction;
			actionContext = new UnitActionContext()
			{
				Target = target
			};
		}

		#endregion

		#region Health

		public override void DealDamage(float damage, BattleObject damageSource)
		{
			base.DealDamage(damage, damageSource);
			health -= damage;
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			currentActionType = UnitActionType.Death;
		}

		#endregion

		#region Unit Transform

		public void RotateTowardTarget(float2 targetPosition, float dT)
		{
			var targetDirection = math.normalize(targetPosition - Position);
			var targetOrientation = MathUtil.ConvertDirectionToOrientation(targetDirection);
			RotateTowardTarget(targetOrientation, dT);
		}

		public void RotateTowardTarget(float targetOrientation, float dT)
		{
			var orientationDiff = Mathf.Abs(Mathf.DeltaAngle(targetOrientation, Orientation));
			var angleDelta = 225 * dT + orientationDiff * 0.05f;
			Orientation = Mathf.MoveTowardsAngle(Orientation, targetOrientation, angleDelta);
		}

		public bool HasLineOfSight(float2 target)
		{
			return true;
		}

		/// <summary>
		/// Adds velocity towards given point.
		/// </summary>
		public void AddVelocityTowards(float2 launchPoint, float velocityMagnitude)
		{
			var dirToPoint = math.normalize(launchPoint - Position);
			Velocity += dirToPoint * velocityMagnitude;
		}

		/// <summary>
		/// Moves to position, but checs board edges.
		/// </summary>
		public void MoveToPosition(float2 newPosition)
		{
			Position = GameWorld.Board.ClampPosition(newPosition, Radius);
		}

		public bool IsWithinAttackRange(UnitTargetInfo other)
		{
			return IsWithinRange(other, other.Radius + Settings.PrimaryAttack.AttackRange);
		}

		public bool IsWithinRange(UnitTargetInfo other, float range)
		{
			return distancesq(Position, other.Position) <= range * range;
		}

		public void SetBrain(IBrain newBrain)
		{
			brain = newBrain;
		}

		#endregion
	}

	public struct UnitActionContext
	{
		public UnitTargetInfo Target;
		public bool Started;
		public float Progress;
		public bool Executed;
		public bool Finished;

		public void ResetProgress()
		{
			Progress = 0;
			Executed = false;
			Finished = false;
			Started = false;
		}
	}
}