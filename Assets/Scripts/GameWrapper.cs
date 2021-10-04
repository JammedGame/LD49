using System;
using Game.Simulation;
using Game.Simulation.Board;
using Game.UI;
using Game.View;
using Game.View.SpellView;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Game
{
	public class GameWrapper : MonoBehaviour
	{
		// scene references
		public CameraController CameraController;
		public HealthBarController HealthBarController;
		public InGameUIController GameUIController;
		public GameWorldData GameData;

		// state
		GameWorld gameWorld;
		GameViewController viewController;
		GameTimeline gameTimeline;
		GameInputController inputController;
		private SelectionCircle selectionCircle;
		private MovementCross movementCross;

		// properties
		public Camera Camera => CameraController.Camera;
		public GameWorld GameWorld => gameWorld;
		public GameViewController ViewController => viewController;
		public BoardData Board => gameWorld?.Board;
		public Unit SelectedUnit => inputController?.SelectedUnit;

		void OnEnable()
		{
			// update unity stuff
			QualitySettings.vSyncCount = 1;
			Application.targetFrameRate = -60;

			// create new game world
			selectionCircle = Instantiate(SelectionCircle.LoadPrefab());
			movementCross = Instantiate(MovementCross.LoadPrefab());
			viewController = new GameViewController(HealthBarController, CameraController, GameUIController, selectionCircle, movementCross);
			gameWorld = new GameWorld(GameData, viewController);
			gameTimeline = new GameTimeline();
			inputController = new GameInputController(this);

			// update camera
			GameUIController.Initialize(this);
		}

		// void OnGUI()
		// {
		// 	GUI.Label(new Rect(20, 20, 100, 100), $"{Screen.width}x{Screen.height} FPS:{1f / Time.deltaTime}");
		// }

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

			// update camera
			if (SelectedUnit is Unit selectedUnit)
			{
				CameraController.Position = selectedUnit.GetCenterPosition3D();
			}
			CameraController.UpdateCamera();

			// update view
			viewController.Update(GameWorld, Time.deltaTime);
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