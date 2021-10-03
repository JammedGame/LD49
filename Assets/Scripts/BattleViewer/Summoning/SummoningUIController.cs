using System.Collections.Generic;
using Game.Simulation;
using UnityEngine;

public class SummoningUIController : MonoBehaviour
{
	[SerializeField] private List<SummoningOptionView> buttons;

	private List<SummoningOption> data;

	public void SetData(List<SummoningOption> newData)
	{
		data = newData;
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		for (var i = 0; i < buttons.Count; i++)
		{
			var button = buttons[i];
			if (data != null && i < data.Count)
			{
				var unitSettings = data[i];
				button.SetData(unitSettings);
				button.gameObject.SetActive(true);
			}
			else
			{
				button.gameObject.SetActive(false);
			}
		}
	}
}