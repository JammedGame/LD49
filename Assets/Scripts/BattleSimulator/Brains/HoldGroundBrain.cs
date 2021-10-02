using Game.Simulation;

namespace BattleSimulator.Brains
{
	public class HoldGroundBrain : IBrain
	{
		private readonly Unit myUnit;

		public HoldGroundBrain(Unit myUnit)
		{
			this.myUnit = myUnit;
		}

		public Decision Think()
		{
			if (myUnit.CurrentActionType == UnitActionType.Idle)
			{
				var target = PickHighestAggroTargetInRange();
				if (target != null) return new Decision(myUnit.Settings.PrimaryAttack, target);
			}

			return null;
		}

		private Unit PickHighestAggroTargetInRange()
		{
			Unit target = null;
			var maxAggro = 0f;
			foreach (var candidate in myUnit.GameWorld.AllUnits)
				if (candidate.IsValidAttackTarget &&
					myUnit.Owner != candidate.Owner && myUnit.IsWithinRange(candidate))
				{
					var aggro = CalculateAggro(candidate);
					if (aggro > maxAggro)
					{
						target = candidate;
						maxAggro = aggro;
					}
				}

			return target;
		}

		private float CalculateAggro(Unit other)
		{
			var aggro = other.Settings.PrimaryAttack.Damage / other.Settings.Health;
			if (other.CurrentTarget.TargetObject == myUnit) aggro *= 1000f;
			return aggro;
		}
	}
}