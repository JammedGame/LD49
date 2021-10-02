using Unity.Mathematics;

namespace Game.Simulation
{
	public class Creep : Unit
	{
		public Creep(GameWorld gameWorld, UnitSettings unitSettings, float2 position, OwnerId owner,
			BattleObject parent) : base(gameWorld, unitSettings, position, owner, parent)
		{
		}
	}
}