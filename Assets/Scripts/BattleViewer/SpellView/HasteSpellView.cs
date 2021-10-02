using Game.Simulation;
using UnityEngine;

namespace Game.View.SpellView
{
    public class HasteSpellView : SpellView
    {
        public override void OnInitialized(BattleObject data)
        {
            Debug.Log("Haste Spell View On Initialize");
        }
    }
}