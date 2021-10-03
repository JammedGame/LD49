using System.Collections.Generic;
using Game.Simulation;
using UnityEngine;

public class SummoningUIController : MonoBehaviour
{
	[SerializeField] private List<SummoningButton> buttons;

	private List<UnitSettings> model;

	private void Start()
	{
		SetModel(new List<UnitSettings>());
	}

	public void SetModel(List<UnitSettings> newModel)
	{
		model = newModel;
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		for (var i = 0; i < buttons.Count; i++)
		{
			var button = buttons[i];
			if (i < model.Count)
			{
				var unitSettings = model[i];
				button.SetModel(unitSettings);
				button.gameObject.SetActive(true);
			}
			else
			{
				button.gameObject.SetActive(false);
			}
		}
	}
}