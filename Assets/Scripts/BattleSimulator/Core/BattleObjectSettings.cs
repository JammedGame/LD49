using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
	public abstract class BattleObjectSettings : ScriptableObject
	{
		public abstract BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null);
	}
}