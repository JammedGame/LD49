using System;
using System.Collections;
using System.Collections.Generic;
using Game.Simulation;
using Game.View;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Image BackgroundImage;
	public Image ProgressImage;

	UnitView unitView;

	public void Initialize(UnitView unitView)
	{
		gameObject.SetActive(true);
		this.unitView = unitView;
	}

	public void Sync()
	{
		if (!unitView)
			return;

		var unit = unitView.Unit;
		if (unit == null)
			return;

		ProgressImage.fillAmount = unit.HealthPercent;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		this.unitView = null;
	}
}
