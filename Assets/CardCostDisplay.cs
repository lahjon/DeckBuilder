using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCostDisplay : MonoBehaviour
{
    public Image energyHolder;
    public Image energyCenter;
    public TMP_Text lblEnergy;

    public void UpdateLabel(string s)
    {
        lblEnergy.text = s;
    }

    public void SetType(EnergyType type)
    {
        switch(type){
            case EnergyType.Standard:
                energyHolder.color = Color.yellow;
                break;
            case EnergyType.Rage:
                energyHolder.color = Color.red;
                break;
        }
    }
}
