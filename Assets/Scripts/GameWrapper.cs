using System;
using Game.Simulation;
using Game.Simulation.Board;
using Game.UI;
using Game.View;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game
{
	public class GameWrapper : MonoBehaviour
	{
		// scene references
		public CameraController CameraController;
		public bool FollowSelectedUnit;
		public GameWorldData GameData;

		// state
		GameWorld gameWorld;
		GameViewController viewController;
		GameTimeline gameTimeline;
		GameInputController inputController;
		int lastBoardMeshSettingsVersion = -1;

		// properties
		public Camera Camera => CameraController.Camera;
		public GameWorld GameWorld => gameWorld;
		public GameViewController ViewController => viewController;
		public BoardData Board => gameWorld?.Board;
		public Unit SelectedUnit => inputController?.SelectedUnit;

		void OnEnable()
		{
			// update unity stuff
			Application.targetFrameRate = 60;

			// create new game world
			viewController = new GameViewController();
			gameWorld = new GameWorld(GameData, viewController);
			gameTimeline = new GameTimeline();
			inputController = new GameInputController(this);

			// update camera
			CameraController.MinX = gameWorld.Board.MinX;
			CameraController.MaxX = gameWorld.Board.MaxX;
			CameraController.MinZ = gameWorld.Board.MinY;
			CameraController.MaxZ = gameWorld.Board.MaxY;
			CameraController.SetCentralPosition();
		}

		void Update()
		{
			if (gameWorld == null)
				return;

			// update input.
			inputController.Update();

			// advance timeline with real unity time.
			gameTimeline.ApplyUndeterministicTime(Time.deltaTime);

			// apply ready ticks
			while(gameTimeline.GetNextTick() is GameTick nextTick)
			{
				gameWorld.Tick(nextTick);
			}

			// update view
			viewController.Update(Time.deltaTime);

			// update camera
			if (FollowSelectedUnit && SelectedUnit is Unit selectedUnit)
				CameraController.Follow(viewController.GetView(selectedUnit).transform);
		}

		public bool TryMouseRaycast(out float2 hitResult)
		{
			if (gameWorld == null)
			{
				hitResult = default;
				return false;
			}

			// get camera ray info
			var ray = Camera.ScreenPointToRay(Input.mousePosition);
			var bottomPlane = new Plane(new Vector3(0, 1, 0), new Vector3(0, 0, 0));

			if (bottomPlane.Raycast(ray, out float distanceToBottom))
			{
				hitResult = ViewUtil.ConvertTo2D(ray.GetPoint(distanceToBottom));
				return true;
			}
			else
			{
				hitResult = default;
				return false;
			}
		}

		public void OnDestroy()
		{
			gameWorld?.Dispose();
		}

#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (gameWorld == null)
				return;

			GizmoService.Draw(this);
		}
#endif
	}
}