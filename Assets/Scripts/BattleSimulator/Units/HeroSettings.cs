using Unity.Mathematics;

namespace Game.Simulation
{
	public class HeroSettings : UnitSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return new Hero(gameWorld, this, targetInfo.Position, owner, parent);
		}
	}

	public class Hero : Unit
	{
		public Hero(GameWorld gameWorld, HeroSettings unitSettings, float2 position, OwnerId owner,
			BattleObject parent) : base(gameWorld, unitSettings, position, owner, parent)
		{
		}
	}
}