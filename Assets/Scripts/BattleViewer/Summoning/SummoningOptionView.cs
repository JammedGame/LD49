using Game.Simulation;
using UnityEngine;
using UnityEngine.UI;

public class SummoningOptionView : MonoBehaviour
{
	[SerializeField] private Image filler;

	[SerializeField] private Text summoningNameDisplay;

	[SerializeField] private Text goldCostLabel;

	private SummoningOption data;

	public void SetData(SummoningOption newData)
	{
		if (newData == null) return;

		data = newData;
		filler.fillAmount = data.IsAvailable ? 1 : 0;
		filler.color = Color.white;
		summoningNameDisplay.text = data.UnitSettings.UnitName;
		goldCostLabel.text = data.UnitSettings.GoldCost > 0 ? $"{data.UnitSettings.GoldCost.ToString()}G" : "FREE";
	}
}