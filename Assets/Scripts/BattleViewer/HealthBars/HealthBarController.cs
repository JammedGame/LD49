using System.Collections;
using System.Collections.Generic;
using Game.Simulation;
using Game.View;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
	public HealthBar HealthBarPrefab;

	Stack<HealthBar> healthBarPool = new Stack<HealthBar>();

	public void SyncHealthbar(BattleObjectView unitView, ref HealthBar healthBar)
    {
		bool shouldHaveHealthBar = unitView != null
            && unitView.Data is Unit unit
            && unit.IsValidAttackTarget;

        if (shouldHaveHealthBar)
        {
			if (healthBar == null)
			{
				healthBar = FetchInstance();
			}
			healthBar.Sync(unitView);
		}
        else
        {
            if (healthBar != null)
            {
                Dispose(healthBar);
                healthBar = null;
            }
        }
	}

	private HealthBar FetchInstance()
	{
		while (healthBarPool.Count > 0)
		{
			HealthBar instance = healthBarPool.Pop();
			if (instance != null)
				return instance;
		}

        return Instantiate(HealthBarPrefab, transform);
	}

	public void Dispose(HealthBar healthBar)
    {
        if (!healthBar)
			return;

		healthBar.Hide();
		healthBarPool.Push(healthBar);
	}
}
