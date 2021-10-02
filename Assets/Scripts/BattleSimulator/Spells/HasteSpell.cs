using Game.Simulation;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public class HasteSpell : Spell
    {
        public HasteSpell(HasteSpellSettings settings, GameWorld gameWorld, OwnerId owner, BattleObject parent = null) : base(settings, gameWorld, owner, parent)
        {
            
        }

        public override void Execute(Unit caster, UnitTargetInfo targetInfo)
        {
            Debug.Log($"{caster.Settings.name} casts Haste Spell!");
        }
    }
}