using Unity.Mathematics;

namespace Game.Simulation
{
	public class CreepSettings : UnitSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return new Creep(gameWorld, this, targetInfo.Position, owner, parent);
		}
	}

	public class Creep : Unit
	{
		public Creep(GameWorld gameWorld, CreepSettings unitSettings, float2 position, OwnerId owner,
			BattleObject parent) : base(gameWorld, unitSettings, position, owner, parent)
		{
		}
	}
}