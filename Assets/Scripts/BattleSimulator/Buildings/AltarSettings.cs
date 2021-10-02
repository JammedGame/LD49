using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public class AltarSettings : BuildingSettings
	{
		public override BattleObject Spawn(GameWorld gameWorld, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
		{
			return new Atlar(gameWorld, this, targetInfo.Position, owner, parent);
		}
	}

	public class Atlar : Building
	{
		public Atlar(GameWorld gameWorld, BuildingSettings settings, float2 position, OwnerId owner, BattleObject parent) :
			base(gameWorld, settings, position, owner, parent)
		{
		}
	}
}