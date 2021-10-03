using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
    [Serializable]
    [CreateAssetMenu]
    public class WaveData : ScriptableObject
    {
        public int goldReward;
        public List<MultiSpawn> multiSpawns;
    }

    [Serializable]
    public class MultiSpawn
    {
        public float delay;
        [Table] public List<UnitSpawn> unitSpawns;
    }
}