using System;
using System.Collections.Generic;
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
		public readonly GameWorldData Data;
		public readonly BoardData Board;
		readonly IViewEventHandler viewBridge;
		readonly GameWorldPhysics physicsRunner;
		float currentTime;

		// collections of battle objects.
		public readonly List<Projectile> AllProjectiles = new List<Projectile>();
		public readonly List<BattleObject> AllBattleObjects = new List<BattleObject>();
		public readonly List<Unit> AllUnits = new List<Unit>();
		public readonly List<Building> AllBuildings = new List<Building>();

		/// <summary>
		/// Current simulation time.
		/// </summary>
		public float CurrentTime => currentTime;

		/// <summary>
		/// API for the collision and physics.
		/// </summary>
		public GameWorldPhysics Physics => physicsRunner;

		public GameWorld(GameWorldData data, IViewEventHandler viewBridge)
		{
			this.Data = data;
			this.Board = data.Board;
			this.viewBridge = viewBridge;
			this.physicsRunner = new GameWorldPhysics();

			// spawn initial object
			foreach (var unitSpawn in data.UnitSpawns)
				unitSpawn.Execute(this);
		}

		public Unit SpawnUnit(UnitSettings settings, float2 position, OwnerId owner, BattleObject parent = null)
		{
			var newUnit = new Unit(this, settings, position, owner, parent);
			AllUnits.Add(newUnit);
			AllBattleObjects.Add(newUnit);
			DispatchViewEvent(newUnit, ViewEventType.Created);
			return newUnit;
		}

		public Building SpawnBuilding(BuildingSettings settings, float2 position, OwnerId owner, BattleObject parent = null)
		{
			var newObject = new Building(this, settings, position, owner, parent);
			AllUnits.Add(newObject);
			AllBuildings.Add(newObject);
			AllBattleObjects.Add(newObject);
			DispatchViewEvent(newObject, ViewEventType.Created);
			return newObject;
		}

		public Projectile SpawnProjectile(BattleObject source, Vector3 position, Vector3 velocity)
		{
			var newObject = new Projectile(source, position, velocity);
			AllProjectiles.Add(newObject);
			AllBattleObjects.Add(newObject);
			DispatchViewEvent(newObject, ViewEventType.Created);
			DispatchViewEvent(source, ViewEventType.ProjectileFired, newObject);
			return newObject;
		}
		

		public void Tick(GameTick tick)
		{
			// reset modifiers
			ResetModifiers();

			// update projectiles.
			Profiler.BeginSample("Tick Projectiles");
			foreach (var projectile in AllProjectiles)
			{
				projectile.Tick();
			}
			Profiler.EndSample();

			// update units.
			Profiler.BeginSample("Tick Units");
			foreach(var unit in AllUnits)
			{
				unit.Tick();
			}
			Profiler.EndSample();

			// update collision
			Profiler.BeginSample("Tick Physics");
			physicsRunner.Run(this, GameTick.TickDuration);
			Profiler.EndSample();

			CleanInactiveObjects();

			// update time
			currentTime += GameTick.TickDuration;
		}

		public void ResetModifiers()
		{
			foreach(var unit in AllUnits)
			{
				unit.ResetModifiers();
			}
		}

		public void DispatchViewEvent(BattleObject parent, ViewEventType type, object data = null)
		{
			viewBridge?.OnViewEvent(new ViewEvent(parent, type, data));
		}

		public void CleanInactiveObjects()
		{
			// deactivate units with not enough health.
			foreach(var unit in AllUnits)
			{
				if (unit.HealthPercent <= 0f)
				{
					unit.Deactivate();
				}
			}

			// clean deactivated objects.
			for (int i = 0; i < AllBattleObjects.Count; i++)
			{
				BattleObject obj = AllBattleObjects[i];
				if (!obj.IsActive)
				{
					AllBattleObjects.RemoveAt(i--);
					if (obj is Projectile p) AllProjectiles.Remove(p);
					if (obj is Unit u) AllUnits.Remove(u);
					if (obj is Building b) AllBuildings.Remove(b);
				}
			}
		}

		public void Dispose()
		{
			physicsRunner.Dispose();
		}
	}
}