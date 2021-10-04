using System;
using System.Collections.Generic;
using Game.Simulation;
using UnityEngine;

namespace Game.View.SpellView
{
    public class SpellUIController : MonoBehaviour
    {
        public List<EquippedSpellView> equippedSpellViews;

        public void Sync()
        {
            foreach (EquippedSpellView view in equippedSpellViews)
            {
                view.Sync();
            }
        }

		public void SyncSpellUI(Unit player)
		{
			for (int i = 0; i < player.EquippedSpells.Count; i++)
			{
				equippedSpellViews[i].gameObject.SetActive(true);
				equippedSpellViews[i].SetModel(player.EquippedSpells[i]);
			}

			for (int i = player.EquippedSpells.Count; i < equippedSpellViews.Count; i++)
			{
				equippedSpellViews[i].gameObject.SetActive(false);
			}
		}
    }
}