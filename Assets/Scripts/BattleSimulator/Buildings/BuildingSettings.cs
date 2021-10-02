using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class BuildingSettings : UnitSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, float2 position, OwnerId owner, BattleObject parent)
		{
			return gameWorld.SpawnBuilding(this, position, owner);
		}
	}
}