using Game.Simulation;

namespace BattleSimulator.Brains
{
	public interface IBrain
	{
		Decision Think(Unit unit);
	}
}