using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
    [Serializable]
    [CreateAssetMenu]
    public class WaveData : ScriptableObject
    {
        public string name;
        public List<MultiUnitSpawnWithDelay> multiUnits;
    }

    [Serializable]
    public class MultiUnitSpawnWithDelay
    {
        public float delay;
        public List<UnitSpawn> unitSpawns;

    }
}