using System.Collections;
using System.Collections.Generic;
using BattleSimulator.Spells;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSpellView : MonoBehaviour
{
    [SerializeField]
    private Image filler;
    
    [SerializeField]
    private Text spellNameDisplay;
    
    private EquippedSpell model;

    public void SetModel(EquippedSpell newModel)
    {
        model = newModel;
        spellNameDisplay.text = model.SpellSettings.spellName;
    }
    
    public void Sync()
    {
        if (model == null)
        {
            return;
        }
        
        filler.fillAmount = model.CooldownProgress;
        Color color = Color.white;
        if (filler.fillAmount < 1f)
        {
            color.a = 0.4f;
        }
        else
        {
            filler.fillAmount = 1f;
        }
        filler.color = color;
    }
}
