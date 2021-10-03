using Game.Simulation;
using UnityEngine;

namespace BattleSimulator.Spells
{
    [CreateAssetMenu]
    public class ImpaleSpellSettings : SpellSettings
    {
        public float damage;
        public float protrudeSeconds; // how long does it take for the spikes to fully extend
        public float protrudeHoldSeconds;
        public float spikeUpswing; // protrude progress % where damage is dealt
        public float aoeRadius;
        public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
        {
            return new ImpaleSpell(parent, targetInfo.Position, this, world);
        }
    }
}