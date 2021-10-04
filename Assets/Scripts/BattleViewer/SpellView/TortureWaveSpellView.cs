using System;
using BattleSimulator.Spells;
using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.View.SpellView
{
    public class TortureWaveSpellView : SpellView
    {
        private TortureWaveSpell spell;
        [SerializeField] private Transform tentacleTransform;
        [SerializeField] private SkinnedMeshRenderer tentacleMeshRenderer;
        
        private float tentacleLength;
        
        public override void OnInitialized(BattleObject data)
        {
            spell = data as TortureWaveSpell;
            tentacleLength = tentacleMeshRenderer.bounds.size.x * 1.4f;
            transform.position = spell.GetPosition3D() + Vector3.up;
            transform.LookAt(transform.position + spell.coneDirection3D);
        }

        public override void SyncView(float dT)
        {
            base.SyncView(dT);
            float scale = spell.coneLength / tentacleLength;
            tentacleTransform.localScale = new Vector3(scale, spell.growProgress, spell.growProgress);
        }

        // private void OnDrawGizmos()
        // {
        //     if (data == null)
        //     {
        //         return;
        //     }
        //     Gizmos.color = Color.green;
        //     Vector3 conePos = data.GetPosition3D();
        //     Gizmos.DrawLine(conePos, spell.coneCenterEnd3D);
        //     Gizmos.DrawLine(conePos, spell.coneLeftEnd3D);
        //     Gizmos.DrawLine(conePos, spell.coneRightEnd3D);
        // }
    }
}