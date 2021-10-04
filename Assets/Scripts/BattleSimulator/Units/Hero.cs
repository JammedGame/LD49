using BattleSimulator.Brains;
using BattleSimulator.Spells;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Simulation
{
    public class Hero : Unit
    {
        public Hero(GameWorld gameWorld, HeroSettings unitSettings, float2 position, OwnerId owner,
            BattleObject parent) : base(gameWorld, unitSettings, position, owner, parent)
        {
            SetBrain(HeroAggroBrain.Instance);
            gameWorld.DispatchViewEvent(this, ViewEventType.PlayerSpellsUpdated);
        }

        public override void OrderSpellCast(EquippedSpell spell, UnitTargetInfo targetInfo)
        {
            base.OrderSpellCast(spell, targetInfo);

            GameWorld.WildMagicController.OnSpellCast(spell.SpellSettings);
        }
    }
}