using Game.Simulation;
using UnityEngine;
using UnityEngine.Serialization;

namespace BattleSimulator.Spells
{
    [CreateAssetMenu]
    public class TortureWaveSpellSettings : SpellSettings
    {
        public float coneLength;
        public float coneAngle;
        public float coneGrowSeconds;
        public float holdSeconds;
        public float damage;
        public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId ownerId, BattleObject parent = null)
        {
            return new TortureWaveSpell(parent, this, world, targetInfo.Position);
        }
    }
}