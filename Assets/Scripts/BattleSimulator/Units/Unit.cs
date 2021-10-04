using System;
using System.Collections.Generic;
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
		public Stat MinAttackDamage;
		public Stat MaxAttackDamage;

		// transform state
		public float2 Position;
		public float2 Velocity;
		public float Orientation;

		// actions
		public readonly UnitMoveWithPathfinding MovementAction = new UnitMoveWithPathfinding();

		// API
		public float CurrentActionProgress => actionContext.Progress;
		public bool CurrentActionExecuted => actionContext.Executed;
		public UnitActionType CurrentActionType => currentActionType;
		public UnitTargetInfo CurrentTarget => actionContext.Target;
		public UnitAction CurrentAction => currentAction;
		public float2 GetDirection() => MathUtil.ConvertOrientationToDirection(Orientation);
		public bool IsAttacking => currentActionType == UnitActionType.Attack;
		public bool IsAttackingUnit(Unit rhs) => currentActionType == UnitActionType.Attack && CurrentTarget.TargetUnit == rhs;
		public bool IsMoving => currentActionType == UnitActionType.Movement;
		public float Radius => Settings.Size;
		public float Health => health;
		public bool IsInvulnerable => isInvulnerable;
		public float HealthPercent => Mathf.Clamp01(health / Settings.Health);
		public virtual bool IsStatic => false; // for collision purposes
		public bool IsValidAttackTarget => IsActive && !IsInvulnerable;
		public bool CanAttack(Unit unit) => unit != null && unit.IsValidAttackTarget && unit.Owner != Owner;
		public float GetRandomDamage() => Mathf.RoundToInt(UnityEngine.Random.Range(MinAttackDamage, MaxAttackDamage));

		public override string ViewPath => $"View/UnitViews/{Settings.name}View";

		private IBrain brain;

		public override Vector3 GetPosition3D() => GameWorld.Board.GetPosition3D(Position);
		public override Vector3 GetCenterPosition3D() => GameWorld.Board.GetPosition3D(Position) + new Vector3(0, Settings.Height / 2, 0f);
		public override float2 GetPosition2D() => Position;

		public List<EquippedSpell> EquippedSpells = new List<EquippedSpell>();

		public Unit(GameWorld gameWorld, UnitSettings unitSettings, float2 position, OwnerId owner, BattleObject parent)
			: base(gameWorld, owner, parent)
		{
			currentAction = DoNothingAction.Instance;
			health = unitSettings.Health;
			Settings = unitSettings;
			Position = position;
			Speed = unitSettings.Speed;
			MinAttackDamage = unitSettings.PrimaryAttack.MinDamage;
			MaxAttackDamage = unitSettings.PrimaryAttack.MaxDamage;
			brain = DefaultAggroBrain.Instance;
			ResetModifiers();

			// load spells
			foreach (SpellSettings spellSettings in Settings.Spells)
			{
				EquippedSpells.Add(new EquippedSpell(spellSettings, this));
			}
		}

		public virtual void ResetModifiers()
		{
			isInvulnerable = Settings.IsInvulnerable;
			Speed.Reset();
			MinAttackDamage.Reset();
			MaxAttackDamage.Reset();
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

			// tick equipped spells
			foreach (EquippedSpell spell in EquippedSpells)
			{
				spell.Tick();
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
			if (attackTarget.TargetUnit != null && !CanAttack(attackTarget.TargetUnit))
			{
				Debug.LogWarning($"Skipped attacking {attackTarget}");
				return;
			}

			StartAction(Settings.PrimaryAttack, attackTarget);
		}

		public virtual void OrderSpellCast(EquippedSpell spell, UnitTargetInfo targetInfo)
		{
			StartAction(new CastSpellAction(spell, Settings.CastUpswing), targetInfo);
		}

		public void OrderSpellCast(int spellIndex, UnitTargetInfo targetInfo)
		{
			if (spellIndex >= EquippedSpells.Count)
			{
				Debug.Log($"No spell at spell index {spellIndex}!");
				return;
			}

			EquippedSpell spell = EquippedSpells[spellIndex];
			if (spell.IsReady)
			{
				OrderSpellCast(spell, targetInfo);
			}
			else
			{
				Debug.Log($"Can't cast spell {spell.SpellSettings.spellName}, {spell.CooldownSecondsLeft}s of cooldown left!");
			}
		}

		public void StartAction(UnitAction newAction, UnitTargetInfo target = default)
		{
			if (newAction == null)
				throw new NullReferenceException();

			// don't reset progress if animation is the same.
			if (currentAction == newAction)
			{
				actionContext.Target = target;
			}
			else
			{
				currentAction = newAction;
				actionContext = new UnitActionContext() { Target = target };
			}
		}

		#endregion

		#region Health

		public override void DealDamage(float damage, BattleObject damageSource)
		{
			base.DealDamage(damage, damageSource);
			health -= damage;
			brain?.OnDamageReceived(this, damageSource, damage);
		}

		public void Die()
		{
			Deactivate();
			SpawnOnDeath();
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			currentActionType = UnitActionType.Death;
		}

		private void SpawnOnDeath()
		{
			if (Settings.SpawnOnDeath != null) GameWorld.ScheduleSpawn(Settings.SpawnOnDeath, Position, Owner, Parent);
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