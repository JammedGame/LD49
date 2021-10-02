using Game.Simulation;
using Unity.Mathematics;

namespace BattleSimulator.Brains
{
	public class AggroAltarBrain : IBrain
	{
		public const float DefaultAggroRange = 7;
		public const float BreakAggroRange = 11;

		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType == UnitActionType.Idle
			|| myUnit.CurrentTarget.TargetObject == myUnit.GameWorld.Altar
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
			Unit target = null;
			float aggroRange = math.max(DefaultAggroRange, myUnit.Settings.PrimaryAttack.AttackRange);
			var maxAggro = 0f;
			foreach (var candidate in myUnit.GameWorld.AllUnits)
				if (candidate.IsValidAttackTarget
				&& myUnit.Owner != candidate.Owner
				&& myUnit.IsWithinRange(candidate, aggroRange))
				{
					var aggro = CalculateAggro(myUnit, candidate);
					if (aggro > maxAggro)
					{
						target = candidate;
						maxAggro = aggro;
					}
				}

			return target ?? myUnit.GameWorld.Altar;
		}

		private float CalculateAggro(Unit myUnit, Unit other)
		{
			var aggro = other.Settings.PrimaryAttack.Damage / other.Settings.Health;
			if (other.CurrentTarget.TargetObject == myUnit) aggro *= 1000f;
			return aggro;
		}
	}
}