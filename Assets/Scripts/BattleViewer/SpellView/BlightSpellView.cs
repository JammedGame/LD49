using BattleSimulator.Spells;
using Game.Simulation;
using UnityEngine;

namespace Game.View.SpellView
{
    public class BlightSpellView : SpellView
    {
        public GameObject DeathCoilPrefab;
        private Transform ActiveDeathCoil;
        public override void OnInitialized(BattleObject data)
        {
            
        }
    }
}