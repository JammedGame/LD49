using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    [CreateAssetMenu]
    public class BlightSpellSettings : SpellSettings
    {
        public float hopRadius = 2f;
        public int maxHops = 5;
        public float damagePerHop = 1;
        public float damageDecay = 1f; // 0 means full damage loss after first hop, 1 means never losing damage
        public float coilSpeed = 1f;
        public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
        {
            return new BlightSpell(parent, targetInfo, this, world);
        }
    }
}