namespace Game.Simulation
{
	public class HeroSettings : UnitSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return new Hero(gameWorld, this, targetInfo.Position, owner, parent);
		}
	}
}