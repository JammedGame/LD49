using Game.Simulation;

namespace BattleSimulator.Brains
{
	public class HoldGroundBrain : IBrain
	{
		public Decision Think(Unit myUnit)
		{
			if (myUnit.CurrentActionType == UnitActionType.Idle
			|| !myUnit.CurrentTarget.IsValid)
			{
				var target = PickHighestAggroTargetInRange(myUnit);
				if (target != null) return new Decision(myUnit.Settings.PrimaryAttack, target);
			}

			return null;
		}

		private Unit PickHighestAggroTargetInRange(Unit myUnit)
		{
			Unit target = null;
			var maxAggro = 0f;
			foreach (var candidate in myUnit.GameWorld.AllUnits)
				if (candidate.IsValidAttackTarget &&
					myUnit.Owner != candidate.Owner &&
					myUnit.IsWithinAttackRange(candidate))
				{
					var aggro = CalculateAggro(myUnit, candidate);
					if (aggro > maxAggro)
					{
						target = candidate;
						maxAggro = aggro;
					}
				}

			return target;
		}

		private float CalculateAggro(Unit myUnit, Unit other)
		{
			var aggro = other.Settings.PrimaryAttack.Damage / other.Settings.Health;
			if (other.CurrentTarget.TargetObject == myUnit) aggro *= 1000f;
			return aggro;
		}
	}
}