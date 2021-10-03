using BattleSimulator.Spells;
using Game.Simulation;
using UnityEngine;

namespace Game.View.SpellView
{
    public class ImpaleSpellView : SpellView
    {
        [SerializeField]
        private Animation spikeAnimation;

        private ImpaleSpell impaleSpell;
        public override void OnInitialized(BattleObject data)
        {
            impaleSpell = data as ImpaleSpell;
            Vector3 pos = new Vector3(impaleSpell.aoeCenter.x, 0, impaleSpell.aoeCenter.y);
            transform.position = pos;

            spikeAnimation.Play();
            foreach (AnimationState state in spikeAnimation)
            {
                state.speed = 0;
            }
        }

        public override void SyncView(float dT)
        {
            base.SyncView(dT);
            
            foreach (AnimationState state in spikeAnimation)
            {
                state.normalizedTime = impaleSpell.protrudeProgress;
            }
        }
    }
}