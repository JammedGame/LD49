namespace Game.Simulation
{
	public class SummoningOption
	{
		public readonly bool IsAvailable;
		public readonly UnitSettings UnitSettings;

		public SummoningOption(UnitSettings unitSettings, bool isAvailable)
		{
			UnitSettings = unitSettings;
			IsAvailable = isAvailable;
		}
	}
}