using BattleSimulator.Brains;
using Unity.Mathematics;

namespace Game.Simulation
{
    public class Hero : Unit
    {
        public Hero(GameWorld gameWorld, HeroSettings unitSettings, float2 position, OwnerId owner,
            BattleObject parent) : base(gameWorld, unitSettings, position, owner, parent)
        {
            SetBrain(new HoldGroundBrain());
            gameWorld.DispatchViewEvent(this, ViewEventType.PlayerSpellsUpdated);
        }
    }
}