using System.Collections.Generic;
using BattleSimulator.Brains;
using UnityEngine;

namespace Game.Simulation
{
	public class WaveController
	{
		private readonly List<WaveData> waves;
		private readonly GameWorld world;

		private int currentWaveIndex = -1;
		private float prevSpawnTime;

		public WaveController(List<WaveData> waves, GameWorld world)
		{
			this.world = world;
			this.waves = waves;
		}

		public string CurrentWaveName => CurrentWave != null ? CurrentWave.name : null;
		public float TimeSinceStartOfWave { get; private set; }
		public bool AnyWavesRemaining => currentWaveIndex < waves.Count;

		public bool AnySpawnsRemaining => CurrentWave != null &&
			CurrentWave.multiSpawns.Exists(ms => prevSpawnTime < ms.delay);

		private WaveData CurrentWave =>
			currentWaveIndex >= 0 && currentWaveIndex < waves.Count ? waves[currentWaveIndex] : null;

		public void StartNextWave()
		{
			currentWaveIndex++;
			TimeSinceStartOfWave = 0;
			prevSpawnTime = 0;
			Debug.Log($"Starting wave: {CurrentWaveName}");
		}

		public void Tick()
		{
			if (!AnySpawnsRemaining) return;

			foreach (var multiSpawn in CurrentWave.multiSpawns)
				if (multiSpawn.delay <= TimeSinceStartOfWave && multiSpawn.delay > prevSpawnTime)
				{
					// spawn
					ExecuteMultiSpawn(multiSpawn);
					prevSpawnTime = TimeSinceStartOfWave;
				}

			TimeSinceStartOfWave += GameTick.TickDuration;
		}

		private void ExecuteMultiSpawn(MultiSpawn multiSpawn)
		{
			Debug.Log($"Spawning enemies at {TimeSinceStartOfWave}");
			for (var i = 0; i < multiSpawn.unitSpawns.Count; i++)
			{
				// execute
				var unit = multiSpawn.unitSpawns[i].Execute(world);
				unit?.SetBrain(new AggroAltarBrain());
			}
		}
	}
}