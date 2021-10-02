using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class BuildingSettings : UnitSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return gameWorld.SpawnBuilding(this, targetInfo.Position, owner);
		}
	}
}