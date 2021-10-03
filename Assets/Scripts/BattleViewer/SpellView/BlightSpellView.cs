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
            Vector3 pos = new Vector3(SpellData.coilPosition.x, 1.5f, SpellData.coilPosition.y);
            ActiveDeathCoil = Instantiate(DeathCoilPrefab, pos,
                DeathCoilPrefab.transform.rotation).transform;
        }

        public override void SyncView(float dT)
        {
            if (SpellData.currentTarget == null)
            {
                return;
            }
            
            ActiveDeathCoil.position = new Vector3(SpellData.coilPosition.x, 1.5f, SpellData.coilPosition.y);
            ActiveDeathCoil.LookAt(SpellData.currentTarget.GetPosition3D() + Vector3.up * 2f);
        }

        protected override void OnDeactivated()
        {
            Destroy(ActiveDeathCoil.gameObject);
        }
    }
}