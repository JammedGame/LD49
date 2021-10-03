using Game.Simulation;
using Unity.Mathematics;

namespace BattleSimulator.Brains
{
	public class AggroEverythingBrain : IBrain
	{
		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType != UnitActionType.Attack
			|| !myUnit.CurrentTarget.IsValid)
			{
				var target = PickHighestAggroTargetInRange(myUnit);
				if (target != null)
					return new Decision(myUnit.Settings.PrimaryAttack, target);
			}

			return null;
		}

		private Unit PickHighestAggroTargetInRange(Unit myUnit)
		{
			Unit target = null;

			foreach (var candidate in myUnit.GameWorld.AllUnits)
			{
				if (!myUnit.CanAttack(candidate))
					continue;

				if (target == null || IsBetterAggro(myUnit, candidate, target))
				{
					target = candidate;
				}
			}

			return target;
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