using Game.Simulation;

namespace BattleSimulator.Brains
{
	public interface IBrain
	{
		Decision Think(Unit unit);

		void OnDamageReceived(Unit unit, BattleObject fromUnit, float damageAmount);
	}
}