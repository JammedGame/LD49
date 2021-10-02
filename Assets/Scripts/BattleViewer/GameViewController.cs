using System;
using System.Collections.Generic;
using Game.Simulation;
using UnityEngine.Profiling;
using UnityEngine;

namespace Game.View
{
	public class GameViewController : IViewEventHandler
	{
		public readonly HealthBarController HealthBarController;
		public readonly CameraController CameraController;

		readonly Dictionary<BattleObject, BattleObjectView> battleObject2ViewDict = new Dictionary<BattleObject, BattleObjectView>();
		readonly List<BattleObjectView> allBattleViews = new List<BattleObjectView>();
		readonly List<ViewEvent> eventsInQueue = new List<ViewEvent>();

		public GameViewController(HealthBarController healthBarController, CameraController cameraController)
		{
			HealthBarController = healthBarController;
			CameraController = cameraController;
		}

		public void OnViewEvent(ViewEvent evt)
		{
			eventsInQueue.Add(evt);
		}

		public void Update(float dT)
		{
			Profiler.BeginSample("ViewController.ExecuteViewEvents");
				ExecuteViewEvents();
			Profiler.EndSample();

			Profiler.BeginSample("ViewController.SyncViews");
				SyncViews(dT);
			Profiler.EndSample();
		}

		private void SyncViews(float dT)
		{
			foreach(var view in allBattleViews)
			{
				if (!view)
					continue;

				if (view.Data.IsActive)
				{
					view.SyncView(dT);
				}
				else
				{
					view.SyncDeadView(dT);
				}
			}
		}

		public void ExecuteViewEvents()
		{
			foreach(var evt in eventsInQueue)
			{
				try
				{
					BattleObjectView view;

					switch(evt.Type)
					{
						case ViewEventType.Created:
						{
							CreateViewObject(evt.Parent);
							break;
						}

						case ViewEventType.End:
						{
							if (battleObject2ViewDict.TryGetValue(evt.Parent, out view))
								view.Deactivate();
							break;
						}

						default:
						{
							if (battleObject2ViewDict.TryGetValue(evt.Parent, out view))
								view.OnViewEvent(evt);
							break;
						}
					}
				}
				catch(Exception e)
				{
					Debug.LogException(e);
				}
			}

			// finally, clear the qeueu
			eventsInQueue.Clear();
		}

		private void CreateViewObject(BattleObject parent)
		{
			var newView = BattleObjectView.Create(parent, this);
			battleObject2ViewDict[parent] = newView;
			allBattleViews.Add(newView);
		}

		/// <summary>
		/// Returns view linked to this battle object.
		/// </summary>
		public BattleObjectView GetView(BattleObject parent)
		{
			if (battleObject2ViewDict.TryGetValue(parent, out var view))
				return view;

			return null;
		}
	}
}