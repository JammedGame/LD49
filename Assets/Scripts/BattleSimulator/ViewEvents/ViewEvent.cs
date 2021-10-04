using System;
using System.Collections.Generic;

namespace Game.Simulation
{
    public readonly struct ViewEvent
    {
        public readonly BattleObject Parent;
        public readonly ViewEventType Type;
        public readonly object Data;

        public ViewEvent(BattleObject parent, ViewEventType type, object data)
        {
            Parent = parent;
            Type = type;
            Data = data;
        }
    }

    public interface IViewEventHandler
    {
        void OnViewEvent(ViewEvent evt);
    }

    public enum ViewEventType
    {
        Undefined = 0,
        Created = 1,
        End = 2,
        ProjectileFired = 3,
        PlayerSpellsUpdated = 4,
        SummoningListUpdated = 5
    }
}