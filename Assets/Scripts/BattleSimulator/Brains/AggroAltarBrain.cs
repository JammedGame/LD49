using Game.Simulation;
using Unity.Mathematics;

namespace BattleSimulator.Brains
{
	public class AggroAltarBrain : IBrain
	{
		public const float DefaultAggroRange = 7;
		public const float BreakAggroRange = 14;

		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType != UnitActionType.Attack
			|| !myUnit.IsWithinRange(myUnit.CurrentTarget, BreakAggroRange))
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
			Unit target = null;
			float aggroRange = math.max(DefaultAggroRange, myUnit.Settings.PrimaryAttack.AttackRange);
			foreach (var candidate in myUnit.GameWorld.AllUnits)
			{
				if (!myUnit.CanAttack(candidate)) continue;

				if (currentTarget == candidate && myUnit.IsWithinRange(candidate, BreakAggroRange)
				|| myUnit.IsWithinRange(candidate, DefaultAggroRange))
				{
					if (target == null || IsBetterAggro(myUnit, candidate, target))
					{
						target = candidate;
					}
				}
			}

			return target ?? myUnit.GameWorld.Altar;
		}

		private bool IsBetterAggro(Unit myUnit, Unit candidate, Unit currentTarget)
		{
			var candidateIsInRange = myUnit.IsWithinAttackRange(candidate);
			var currentTargetIsInRange = myUnit.IsWithinAttackRange(currentTarget);
			if (candidateIsInRange && !currentTargetIsInRange) return true;
			if (!candidateIsInRange && currentTargetIsInRange) return false;

			return
				math.distancesq(myUnit.Position, candidate.Position)
				< math.distancesq(myUnit.Position, currentTarget.Position);
		}
	}
}