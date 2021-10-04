using Game.Simulation;
using Unity.Mathematics;

namespace BattleSimulator.Brains
{
	public class HeroAggroBrain : IBrain
	{
		public static HeroAggroBrain Instance = new HeroAggroBrain();

		public void OnDamageReceived(Unit unit, BattleObject fromUnit, float damageAmount)
		{
		}

		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType == UnitActionType.Idle)
				return DefaultAggroBrain.Instance.Think(myUnit);

			return null;
		}
	}

	public class DefaultAggroBrain : IBrain
	{
		public static DefaultAggroBrain Instance = new DefaultAggroBrain();

		public const float DefaultAggroRange = 9;
		public const float BreakAggroRange = 15;

		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType != UnitActionType.Attack
			&& myUnit.CurrentActionType != UnitActionType.CastSpell
			&& myUnit.MaxAttackDamage > 0)
			{
				var target = PickHighestAggroTargetInRange(myUnit);
				if (target != null)
					return new Decision(myUnit.Settings.PrimaryAttack, target);
			}

			return null;
		}

		private Unit PickHighestAggroTargetInRange(Unit myUnit)
		{
			Unit currentTarget = myUnit.CurrentTarget.TargetUnit;
			Unit newTarget = currentTarget;
			float aggroRange = math.max(DefaultAggroRange, myUnit.Settings.PrimaryAttack.AttackRange);
			bool canMove = myUnit.Speed > 0;
			foreach (var candidate in myUnit.GameWorld.AllUnits)
			{
				if (!myUnit.CanAttack(candidate)) continue;
				if (!canMove && !myUnit.IsWithinAttackRange(candidate)) continue;

				if (currentTarget == candidate && myUnit.IsWithinRange(candidate, BreakAggroRange)
				|| myUnit.IsWithinRange(candidate, aggroRange))
				{
					if (newTarget == null || IsBetterAggro(myUnit, candidate, newTarget))
					{
						newTarget = candidate;
					}
				}
			}

			if (newTarget == null && myUnit.Owner != myUnit.GameWorld.Altar.Owner)
			{
				return myUnit.GameWorld.Altar;
			}

			return newTarget;
		}

		private bool IsBetterAggro(Unit myUnit, Unit candidate, Unit currentTarget)
		{
			var candidateIsDangerous = candidate.MaxAttackDamage > 0;
			var currentTargetIsDangerous = currentTarget.MaxAttackDamage > 0;
			if (candidateIsDangerous && !currentTargetIsDangerous) return true;
			if (!candidateIsDangerous && currentTargetIsDangerous) return false;

			var candidateIsInRange = myUnit.IsWithinAttackRange(candidate);
			var currentTargetIsInRange = myUnit.IsWithinAttackRange(currentTarget);
			if (candidateIsInRange && !currentTargetIsInRange) return true;
			if (!candidateIsInRange && currentTargetIsInRange) return false;

			return
				math.distancesq(myUnit.Position, candidate.Position)
				< math.distancesq(myUnit.Position, currentTarget.Position);
		}

		public void OnDamageReceived(Unit unit, BattleObject damageSource, float damageAmount)
		{
			Unit attacker = (damageSource as Unit) ?? (damageSource.Parent as Unit);
			if (attacker == null)
				return;

			foreach(var friend in unit.GameWorld.AllUnits)
			{
				if (friend.Owner == unit.Owner && friend.IsWithinRange(unit.Position, 3f))
				{
					if (friend.IsAttacking && friend.CurrentTarget.TargetUnit != null && friend.CurrentTarget.TargetUnit.MaxAttackDamage > 0)
					{
						// already busy - no time for revenge.
						continue;
					}

					// revenge time!
					friend.OrderAttacking(attacker);
				}
			}
		}
	}
}