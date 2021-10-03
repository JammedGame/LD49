using System.Collections.Generic;
using BattleSimulator.Brains;
using UnityEngine;

namespace Game.Simulation
{
	public class WaveController
	{
		private readonly List<Unit> currentWaveUnits = new List<Unit>();
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
		public int CurrentWaveGoldReward => CurrentWave != null ? CurrentWave.goldReward : 0;
		public float TimeSinceStartOfWave { get; private set; }
		public bool AnyWavesRemaining => currentWaveIndex < waves.Count;
		public bool WaveComplete => !AnyUnitsRemaining && !AnySpawnsRemaining;

		private bool AnyUnitsRemaining => currentWaveUnits.Exists(u => u != null && u.IsActive);

		private bool AnySpawnsRemaining =>
			CurrentWave != null && CurrentWave.multiSpawns.Exists(ms => ms.delay > prevSpawnTime);

		private WaveData CurrentWave =>
			currentWaveIndex >= 0 && currentWaveIndex < waves.Count ? waves[currentWaveIndex] : null;

		public void StartNextWave()
		{
			currentWaveUnits.Clear();
			currentWaveIndex++;
			TimeSinceStartOfWave = 0;
			prevSpawnTime = 0;
			Debug.Log($"Starting wave: {CurrentWaveName}");
		}

		public void Tick()
		{
			TimeSinceStartOfWave += GameTick.TickDuration;
			if (AnySpawnsRemaining)
				foreach (var multiSpawn in CurrentWave.multiSpawns)
					if (multiSpawn.delay <= TimeSinceStartOfWave && multiSpawn.delay > prevSpawnTime)
					{
						// spawn
						ExecuteMultiSpawn(multiSpawn);
						prevSpawnTime = TimeSinceStartOfWave;
					}
		}

		private void ExecuteMultiSpawn(MultiSpawn multiSpawn)
		{
			Debug.Log($"Spawning enemies at {TimeSinceStartOfWave}");
			foreach (var unitSpawn in multiSpawn.unitSpawns)
			{
				var unit = unitSpawn.Execute(world);
				unit?.SetBrain(new AggroAltarBrain());
				currentWaveUnits.Add(unit);
			}
		}
	}
}