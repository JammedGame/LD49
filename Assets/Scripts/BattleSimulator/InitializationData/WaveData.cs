using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
    [Serializable]
    [CreateAssetMenu]
    public class WaveData : ScriptableObject
    {
        public List<MultiUnitSpawnWithDelay> multiUnits;
    }

    [Serializable]
    public class MultiUnitSpawnWithDelay
    {
        public float delay;
        [Table] public List<UnitSpawn> unitSpawns;
    }
}