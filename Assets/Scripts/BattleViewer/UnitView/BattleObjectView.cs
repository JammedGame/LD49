using System;
using System.Collections;
using System.Collections.Generic;
using Game.Simulation;
using UnityEngine;

namespace Game.View
{
	public abstract class BattleObjectView : MonoBehaviour
    {
        protected BattleObject data;

        public BattleObject Data => data;

        public static BattleObjectView Create(BattleObject data)
        {
            var viewPath = data.ViewPath;
            if (string.IsNullOrEmpty(viewPath))
                return null;

            var prefab = Resources.Load<BattleObjectView>(viewPath);
            if (prefab == null)
            {
                throw new NullReferenceException($"Failed to find prefab at path {viewPath}");
            }

            var newView = Instantiate(prefab);
            newView.data = data;
            newView.OnInitialized(data);
            newView.SyncView(0f);
            return newView;
        }

        public abstract void OnInitialized(BattleObject data);

        /// <summary>
        /// Called each view tick.
        /// </summary>
        public virtual void SyncView(float dT)
        {
        }

		/// <summary>
		/// Called each view tick after target object has been deactivated.
		/// </summary>
		public virtual void SyncDeadView(float dT)
		{
		}

        /// <summary>
        /// Called on every view event.
        /// </summary>
        public virtual void OnViewEvent(ViewEvent evt)
        {
        }

		/// <summary>
		/// Called on object deactivation.
		/// </summary>
		public virtual void OnDeactivated()
		{
		}
    }
}