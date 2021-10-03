using Unity.Mathematics;

namespace Game.Simulation
{
	public class ScheduledSpawn
	{
		public readonly OwnerId Owner;
		public readonly BattleObject Parent;
		public readonly float2 Position;
		public readonly UnitSettings Settings;

		public ScheduledSpawn(UnitSettings settings, float2 position, OwnerId owner, BattleObject parent)
		{
			Settings = settings;
			Position = position;
			Owner = owner;
			Parent = parent;
		}
	}
}