using System;
using System.Collections.Generic;
using Game.Simulation;
using Game.View.SpellView;
using UnityEngine.Profiling;
using UnityEngine;
using Game.UI;

namespace Game.View
{
	public class GameViewController : IViewEventHandler
	{
		public readonly HealthBarController HealthBarController;
		public readonly CameraController CameraController;
		public readonly InGameUIController UIController;

		private readonly SelectionCircle selectionCircle;
		private readonly MovementCross movementCross;

		readonly Dictionary<BattleObject, BattleObjectView> battleObject2ViewDict = new Dictionary<BattleObject, BattleObjectView>();
		readonly List<BattleObjectView> allBattleViews = new List<BattleObjectView>();
		readonly List<ViewEvent> eventsInQueue = new List<ViewEvent>();

		public GameViewController(HealthBarController healthBarController, CameraController cameraController, InGameUIController gameUIController, SelectionCircle selectionCircle, MovementCross movementCross)
		{
			this.HealthBarController = healthBarController;
			this.CameraController = cameraController;
			this.UIController = gameUIController;
			this.selectionCircle = selectionCircle;
			this.movementCross = movementCross;
		}

		public void OnViewEvent(ViewEvent evt)
		{
			eventsInQueue.Add(evt);
		}

		public void Update(GameWorld model, float dT)
		{
			Profiler.BeginSample("ViewController.ExecuteViewEvents");
				ExecuteViewEvents();
			Profiler.EndSample();

			Profiler.BeginSample("ViewController.SyncViews");
				SyncViews(dT);
			Profiler.EndSample();

			Profiler.BeginSample("UIController.Sync");
				UIController.Sync(model);
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

						case ViewEventType.PlayerSpellsUpdated:
							UIController.SpellUIController.SyncSpellUI(evt.Parent as Unit);
							break;

						case ViewEventType.SummoningListUpdated:
							SyncSummoningUI(evt.Data as List<SummoningOption>);
							break;

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

			// finally, clear the queue
			eventsInQueue.Clear();
		}

		private void CreateViewObject(BattleObject parent)
		{
			var newView = BattleObjectView.Create(parent, this);
			battleObject2ViewDict[parent] = newView;
			allBattleViews.Add(newView);
		}

		private void SyncSummoningUI(List<SummoningOption> summoningList)
		{
			UIController.SummoningUIController.SetData(summoningList);
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

		public void SelectPosition(Vector3 position)
		{
			selectionCircle.Hide();
			movementCross.ShowTemporarily(position);
		}

		public void SelectObject(OwnerId selectionOwner, BattleObjectView targetView)
		{
			var isFriend = targetView.Data.Owner == selectionOwner;
			var isEnemy = targetView.Data.Owner == selectionOwner.GetOpposite();
			movementCross.Hide();
			selectionCircle.Follow(targetView);
			if (isFriend) selectionCircle.ShowFriend();
			else if (isEnemy) selectionCircle.ShowEnemy();
			else selectionCircle.ShowDefault();
		}

		public void SelectObjectForAction(OwnerId selectionOwner, BattleObjectView targetView)
		{
			var isFriend = targetView.Data.Owner == selectionOwner;
			var isEnemy = targetView.Data.Owner == selectionOwner.GetOpposite();
			movementCross.Hide();
			selectionCircle.Follow(targetView);
			if (isFriend) selectionCircle.ShowFriendAction();
			else if (isEnemy) selectionCircle.ShowEnemyAction();
			else selectionCircle.ShowDefault();
		}

		public void UnselectObject()
		{
			selectionCircle.Hide();
		}
	}
}