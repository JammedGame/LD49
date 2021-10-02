using System.Collections;
using System.Collections.Generic;
using Game.View;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
	public HealthBar HealthBarPrefab;

	Stack<HealthBar> healthBarPool = new Stack<HealthBar>();

	public HealthBar Fetch(UnitView unitView)
	{
		HealthBar instance = FetchInstance();
		if (instance != null) instance.Initialize(unitView);
		return instance;
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

		healthBarPool.Push(healthBar);
	}
}
