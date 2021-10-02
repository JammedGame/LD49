using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public abstract class Spell : BattleObject
    {
        public readonly SpellSettings Settings;
        
        protected Spell(BattleObject caster, SpellSettings settings, GameWorld gameWorld) : base(gameWorld, caster.Owner, caster)
        {
            Settings = settings;
        }
        
        public override string ViewPath => $"View/SpellViews/{Settings.name}View";
        public override float2 GetPosition2D()
        {
            return float2.zero;
        }

        public override Vector3 GetPosition3D()
        {
            return Vector3.zero;
        }

        public virtual void Tick()
        {
            
        }
    }
}