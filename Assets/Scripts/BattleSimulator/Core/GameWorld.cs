using System.Collections.Generic;
using BattleSimulator.Brains;
using BattleSimulator.Spells;
using Game.Simulation.Board;
using Game.Simulation.Physics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game.Simulation
{
	public class GameWorld
	{
		public readonly List<BattleObject> AllBattleObjects = new List<BattleObject>();
		public readonly List<Building> AllBuildings = new List<Building>();
		public readonly List<Creep> AllCreeps = new List<Creep>();
		public Altar Altar { get; private set; }

		// collections of battle objects.
		public readonly List<Projectile> AllProjectiles = new List<Projectile>();
		public readonly List<Spell> AllSpells = new List<Spell>();
		public readonly List<Unit> AllUnits = new List<Unit>();
		public readonly BoardData Board;
		public readonly GameWorldData Data;
		public readonly WildMagicController WildMagicController;
		private readonly IViewEventHandler viewBridge;
		private readonly WaveController waveController;
		private readonly List<ScheduledSpawn> scheduledSpawns = new List<ScheduledSpawn>();
		private readonly List<(Creep creep, float time)> graveyard = new List<(Creep, float)>();

		private int goldAmount;

		public GameWorld(GameWorldData data, IViewEventHandler viewBridge)
		{
			Data = data;
			Board = data.Board;
			this.viewBridge = viewBridge;
			Physics = new GameWorldPhysics();
			waveController = new WaveController(Data.WaveData, this);
			goldAmount = data.StartingGold;
			UpdateSummoningList(null);

			// spawn initial object
			foreach (var unitSpawn in data.UnitSpawns)
				unitSpawn.Execute(this);

			WildMagicController = new WildMagicController();
		}


        /// <summary>
        /// Current simulation time.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
		/// API for the collision and physics.
        /// </summary>
        public GameWorldPhysics Physics { get; }

        private void SpawnUnit(ScheduledSpawn ss) => SpawnUnit(ss.Settings, ss.Position, ss.Owner, ss.Parent);

        public Unit SpawnUnit(UnitSettings settings, float2 position, OwnerId owner, BattleObject parent = null)
		{
			Unit newUnit = (Unit)settings.Spawn(this, position, owner, parent);
			AllUnits.Add(newUnit);
			AllBattleObjects.Add(newUnit);
			if (newUnit is Building building) AllBuildings.Add(building);
			if (newUnit is Creep creep) AllCreeps.Add(creep);
			if (newUnit is Altar altar) Altar = altar;
			DispatchViewEvent(newUnit, ViewEventType.Created);
			return newUnit;
		}

		public Projectile SpawnProjectile(BattleObject source, Vector3 position, Vector3 velocity, Unit target,
			float damage)
		{
			var newObject = new Projectile(source, position, velocity, target, damage);
			AllProjectiles.Add(newObject);
			AllBattleObjects.Add(newObject);
			DispatchViewEvent(newObject, ViewEventType.Created);
			DispatchViewEvent(source, ViewEventType.ProjectileFired, newObject);
			return newObject;
		}

		public Spell CastSpell(SpellSettings spellSettings, UnitTargetInfo targetInfo, BattleObject caster)
		{
			var newSpell = (Spell)spellSettings.Spawn(this, targetInfo.Position, caster.Owner, caster);
			AllSpells.Add(newSpell);
			AllBattleObjects.Add(newSpell);
			DispatchViewEvent(newSpell, ViewEventType.Created);
			return newSpell;
		}

		public void Tick(GameTick tick)
		{
			// reset modifiers
			ResetModifiers();

			// update projectiles.
			Profiler.BeginSample("Tick Projectiles");
			foreach (var projectile in AllProjectiles) projectile.Tick();
			Profiler.EndSample();

			// update spells
			Profiler.BeginSample("Tick Spells");
			foreach (var spell in AllSpells) spell.Tick();
			Profiler.EndSample();

			// update units.
			Profiler.BeginSample("Tick Units");
			foreach (var unit in AllUnits) unit.Tick();
			Profiler.EndSample();

			// update collision
			Profiler.BeginSample("Tick Physics");
			Physics.Run(this, GameTick.TickDuration);
			Profiler.EndSample();

			// update waves
			if (waveController.AnyWavesRemaining)
			{
				waveController.Tick();
				if (waveController.WaveComplete)
				{
					AddGold(waveController.CurrentWaveGoldReward);
					waveController.StartNextWave();
				}
			}

			// wild magic
			WildMagicController.Tick(GameTick.TickDuration);

			CleanInactiveObjects();

			// update time
			CurrentTime += GameTick.TickDuration;
		}

		private void AddGold(int amount)
		{
			goldAmount += amount;
			Debug.Log($"Gold amount is now {goldAmount}");
		}

		private void SubtractGold(int amount)
		{
			goldAmount -= amount;
			Debug.Log($"Gold amount is now {goldAmount}");
		}

		public void ResetModifiers()
		{
			foreach (var unit in AllUnits) unit.ResetModifiers();
		}

		public void DispatchViewEvent(BattleObject parent, ViewEventType type, object data = null)
		{
			viewBridge?.OnViewEvent(new ViewEvent(parent, type, data));
		}

		public void CleanInactiveObjects()
		{
			// deactivate units with not enough health.
			foreach (var unit in AllUnits)
				if (unit.HealthPercent <= 0f)
					unit.Die();

			// clean deactivated objects.
			for (var i = 0; i < AllBattleObjects.Count; i++)
			{
				var obj = AllBattleObjects[i];
				if (!obj.IsActive)
				{
					AllBattleObjects.RemoveAt(i--);
					if (obj is Projectile p) AllProjectiles.Remove(p);
					if (obj is Unit u) AllUnits.Remove(u);
					if (obj is Building b) AllBuildings.Remove(b);
					if (obj is Spell s) AllSpells.Remove(s);
					if (obj == Altar) Altar = null;
					if (obj is Creep c)
					{
						AllCreeps.Remove(c);
						graveyard.Add((c, CurrentTime));
					}
				}
			}

			// execute scheduled spawns.
			foreach (var scheduledSpawn in scheduledSpawns) SpawnUnit(scheduledSpawn);
			scheduledSpawns.Clear();
		}

		public void Dispose()
		{
			Physics.Dispose();
		}

		public void ScheduleSpawn(UnitSettings settingsSpawnOnDeath, float2 position, OwnerId owner, BattleObject parent)
		{
			scheduledSpawns.Add(new ScheduledSpawn(settingsSpawnOnDeath, position, owner, parent));
		}

		public List<Creep> GetCreepsThatDiedSince(float time)
		{
			List<Creep> creeps = new List<Creep>();
			for (int i = 0; i < graveyard.Count; i++)
			{
				var (creep, graveTime) = graveyard[i];
				if (creep != null && graveTime >= time)
				{
					creeps.Add(creep);
					graveyard.RemoveAt(i--);
				}
			}
			return creeps;
		}

		public void UpdateSummoningList(Unit selectedOther)
		{
			var summoningList = selectedOther?.Settings.SummoningList;
			if (summoningList == null || summoningList.Count == 0) summoningList = Data.DefaultSummoningList;
			var summoningOptions = summoningList.ConvertAll(us => new SummoningOption(us, us.GoldCost <= goldAmount));
			DispatchViewEvent(selectedOther, ViewEventType.SummoningListUpdated, summoningOptions);
		}

		public void SummonBuilding(UnitSettings buildingToSummon, Unit oldBuilding)
		{
			if (buildingToSummon.GoldCost > goldAmount) return;

			SubtractGold(buildingToSummon.GoldCost);
			oldBuilding.Deactivate();

			SpawnUnit(buildingToSummon, oldBuilding.Position, oldBuilding.Owner, oldBuilding.Parent);
		}

		public void SummonCreep(UnitSettings creepToSummon, float2 targetPosition, OwnerId owner)
		{
			if (creepToSummon.GoldCost > goldAmount) return;

			SubtractGold(creepToSummon.GoldCost);

			SpawnUnit(creepToSummon, targetPosition, owner);
		}
	}
}