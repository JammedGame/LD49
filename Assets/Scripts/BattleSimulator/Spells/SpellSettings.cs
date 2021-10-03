using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace BattleSimulator.Spells
{
    public abstract class SpellSettings : BattleObjectSettings
    {
        public string spellName;
        public float manaCost;
        public float cooldownSeconds = 3f;
        public float castRange;
        public float castDurationSeconds;
        public bool isSingleTarget;
    }
}