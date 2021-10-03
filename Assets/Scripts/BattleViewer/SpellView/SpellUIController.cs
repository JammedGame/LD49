using System.Collections.Generic;
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
    }
}