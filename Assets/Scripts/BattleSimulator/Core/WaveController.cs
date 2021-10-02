using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
    public class WaveController
    {
        public List<WaveData> Waves = new List<WaveData>();
        private GameWorld world;
        private int currentWaveIndex = 0;
        private float currentWaveTimer = 0;
        private float prevSpawnTime = 0;

        public WaveController(List<WaveData> waveData, GameWorld world)
        {
            this.world = world;
            this.Waves = waveData;
        }

        private WaveData GetCurrentWave() { return Waves[currentWaveIndex]; }

        public void Tick()
        {
            if (currentWaveIndex >= Waves.Count)
                return;
            bool AnySpawnsRemaining = false;
            WaveData currWave = GetCurrentWave();
            for (int i = 0; i < currWave.multiUnits.Count; i++)
            {
                if (currentWaveTimer <= currWave.multiUnits[i].delay)
                {
                    AnySpawnsRemaining = true;
                }

                if (currWave.multiUnits[i].delay <= currentWaveTimer && currWave.multiUnits[i].delay > prevSpawnTime)
                {
                    // spawn
                    SpawnWave(currWave.multiUnits[i].unitSpawns);
                    prevSpawnTime = currentWaveTimer;
                }

            }

            if (!AnySpawnsRemaining)
            {
                currentWaveIndex++;
                currentWaveTimer = 0;
                prevSpawnTime = 0;
            }

            currentWaveTimer += GameTick.TickDuration;


        }

        void SpawnWave(List<UnitSpawn> unitSpawns)
        {
            Debug.Log("Spawning enemies...");
            for (int i = 0; i < unitSpawns.Count; i++)
            {
                // execute
                unitSpawns[i].Execute(world);
            }

        }

    }
}
