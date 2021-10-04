using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Simulation;
using UnityEngine;
using UnityEngine.UI;

public class WildMagicUI : MonoBehaviour
{
    private Color lowEnergyColor = new Color(0, 1, 0);
    private Color highEnergyColor = new Color(1, 0, 0);

    [SerializeField] private float unstableBlinkIntervalSeconds = 0.5f;
    [SerializeField] private Text energyBarTitle;
    [SerializeField] private Text unstableCaption;
    [SerializeField] private Image energyBar;


    private float sinceLastBlinkSeconds = 0f;

    private void Start()
    {
        unstableCaption.gameObject.SetActive(false);
    }

    public void Sync(WildMagicController model)
    {
        if (model == null)
        {
            return;
        }

        energyBar.fillAmount = model.EnergyProgress;
        if (energyBar.fillAmount > 0.99f)
        {
            sinceLastBlinkSeconds += GameTick.TickDuration;
            if (sinceLastBlinkSeconds > unstableBlinkIntervalSeconds)
            {
                sinceLastBlinkSeconds = 0f;
                unstableCaption.gameObject.SetActive(!unstableCaption.gameObject.activeSelf);
            }
        }
        else
        {
            unstableCaption.gameObject.SetActive(false);
        }

        Color color = lowEnergyColor * (1 - energyBar.fillAmount) + highEnergyColor * energyBar.fillAmount;
        energyBar.color = color;
    }

}

