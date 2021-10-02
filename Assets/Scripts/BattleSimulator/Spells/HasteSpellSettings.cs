using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    [CreateAssetMenu]
    public class HasteSpellSettings : SpellSettings
    {
        public float EffectDurationSeconds = 10f;
        public float SpeedModifier = 2f;

        public override BattleObject Spawn(GameWorld world, UnitTargetInfo targetInfo, OwnerId owner, BattleObject parent)
        {
            return new HasteSpell(parent, this, world, owner);
        }
    }
}