using Game.Simulation;
using UnityEngine;
using UnityEngine.UI;

public class SummoningButton : MonoBehaviour
{
	[SerializeField] private Text summoningNameDisplay;

	private UnitSettings model;

	public void SetModel(UnitSettings newModel)
	{
		if (newModel == null) return;

		model = newModel;
		summoningNameDisplay.text = model.UnitName;
	}
}