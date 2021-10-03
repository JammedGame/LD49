using System;
using System.Collections;
using System.Collections.Generic;
using Game.Simulation;
using Game.View;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Color player1Color;
	public Color player2Color;
	public Image BackgroundImage;
	public Image ProgressImage;
	public AnimationCurve HealthBarScale;

	BattleObjectView unitView;

	public void Initialize(BattleObjectView unitView)
	{
		gameObject.SetActive(true);
		this.unitView = unitView;
		this.ProgressImage.color = unitView.Data.Owner == OwnerId.Player1
			? player1Color : player2Color;
	}

	public void Sync()
	{
		if (!unitView)
			return;

		var unit = unitView.Data as Unit;
		if (unit == null)
			return;

		ProgressImage.fillAmount = unit.HealthPercent;
		CameraController cameraController = unitView.ViewController.CameraController;
		Camera camera = cameraController.Camera;
		Vector3 worldPosition = unitView.transform.position + unit.Settings.Height * Vector3.up;
		Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);
		RectTransform rect = (RectTransform)transform.parent;
		Vector3 uiPosition = new Vector3(rect.sizeDelta.x * (viewportPosition.x - 0.5f), rect.sizeDelta.y * (viewportPosition.y - 0.5f), 0f);
		transform.localPosition = uiPosition;
		transform.localScale = HealthBarScale.Evaluate(cameraController.Distance) * Vector3.one;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		this.unitView = null;
	}
}
