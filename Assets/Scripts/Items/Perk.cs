using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class Perk : MonoBehaviour
{
    public Image image;
    public Button button;
    public PerkData perkData;
    public Transform tooltipAnchor;
    public TMP_Text perkName;
    bool initialized;
    CharacterClassType characterClassType;
    Effect effect;
    bool appliedEffect;
    bool _activated;
    public bool Activated
    {
        get => _activated;
        set 
        {
            _activated = value;
            if (_activated && (characterClassType == WorldSystem.instance.characterManager.selectedCharacterClassType || characterClassType == CharacterClassType.None)) 
                ActivatePerk();
            else
            {
                DeactivePerk();
                _activated = false;
            }
        }
    }
    public void BindData(PerkData data = null, bool activate = true)
    {   
        if (!initialized)
        {
            if (data != null) perkData = data;
            initialized = true;
            image.sprite = perkData.activeArtwork;
            characterClassType = perkData.characterClassType;
            effect = Effect.GetEffect(gameObject, perkData.effect);
            perkName.text = data.perkName;
            if (activate) Activated = true;
        }
    }

    void ActivatePerk()
    {
        if (!appliedEffect) effect?.AddEffect();

        appliedEffect = true;
        image.sprite = perkData.activeArtwork;
        //button.interactable = true;
        image.material.SetInt("Active", 1);
    }

    void DeactivePerk()
    {
        if (appliedEffect) effect?.RemoveEffect();

        appliedEffect = false;
        image.sprite = perkData.inactiveArtwork;
        //button.interactable = false;
        image.material.SetInt("Active", 0);
        
    }

    public void DebugButton()
    {
        Activated = !Activated;
    }
}
