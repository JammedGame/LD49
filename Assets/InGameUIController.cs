using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Simulation;
using Game.View.SpellView;
using UnityEngine;

public class InGameUIController : MonoBehaviour
{
	public SpellUIController SpellUIController;
	public SummoningUIController SummoningUIController;
	public WildMagicUI WildMagicUIController;

	public GameWrapper GameWrapper { get; private set; }

    public void Initialize(GameWrapper game)
    {
		this.GameWrapper = game;
	}

    public void Sync(GameWorld gameWorld)
    {
		var IsBuildingTime = gameWorld.IsBuildingTime;
		SpellUIController.gameObject.SetActive(!IsBuildingTime);
		WildMagicUIController.gameObject.SetActive(!IsBuildingTime);
		SummoningUIController.gameObject.SetActive(IsBuildingTime);

        if (!IsBuildingTime)
        {
            SpellUIController.Sync();
            WildMagicUIController.Sync(gameWorld.WildMagicController);
        }
	}
}
