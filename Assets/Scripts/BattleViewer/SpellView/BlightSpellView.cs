using BattleSimulator.Spells;
using Game.Simulation;
using UnityEngine;

namespace Game.View.SpellView
{
    public class BlightSpellView : SpellView
    {
        public GameObject DeathCoilPrefab;
        private Transform ActiveDeathCoil;
        private BlightSpell SpellData => data as BlightSpell;
        
        public override void OnInitialized(BattleObject data)
        {
            ActiveDeathCoil = Instantiate(DeathCoilPrefab, SpellData.coilPosition,
                DeathCoilPrefab.transform.rotation).transform;
        }

        public override void SyncView(float dT)
        {
            if (SpellData.currentTarget == null)
            {
                return;
            }
            
            ActiveDeathCoil.position = SpellData.coilPosition;
            ActiveDeathCoil.LookAt(SpellData.currentTarget.GetPosition3D());
        }

        protected override void OnDeactivated()
        {
            Destroy(ActiveDeathCoil.gameObject);
        }
    }
}