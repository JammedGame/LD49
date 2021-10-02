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
	public AnimationCurve HealthBarScale;

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
		CameraController cameraController = unitView.ViewController.CameraController;
		Camera camera = cameraController.Camera;
		Vector3 worldPosition = unitView.transform.position + unitView.Height * Vector3.up;
		Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);
		RectTransform rect = (RectTransform)transform.parent;
		Vector3 uiPosition = new Vector3(rect.sizeDelta.x * viewportPosition.x, rect.sizeDelta.y * viewportPosition.y, 0);
		transform.position = uiPosition;
		transform.localScale = HealthBarScale.Evaluate(cameraController.Distance) * Vector3.one;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		this.unitView = null;
	}
}
