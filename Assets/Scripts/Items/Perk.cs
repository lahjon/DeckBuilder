using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class Perk : Item, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    public Button button;
    public PerkData perkData;
    public Transform tooltipAnchor;
    public TMP_Text perkName;
    bool initialized;
    CharacterClassType characterClassType;
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
    public void BindData(PerkData data = null)
    {   
        if (!initialized)
        {
            if (data != null) perkData = data;
            initialized = true;
            image.sprite = perkData.artwork;
            characterClassType = perkData.characterClassType;
            itemEffect = WorldSystem.instance.itemEffectManager.CreateItemEffect(perkData.itemEffectStruct, this, data.itemName);
            perkName.text = data.itemName;
            Activated = true;
        }
    }

    void ActivatePerk()
    {
        if (!appliedEffect) itemEffect?.AddItemEffect();

        appliedEffect = true;
        image.sprite = perkData.artwork;
        //button.interactable = true;
        image.material.SetInt("Active", 1);
    }

    void DeactivePerk()
    {
        if (appliedEffect) itemEffect?.RemoveItemEffect();

        appliedEffect = false;
        image.sprite = perkData.inactiveArtwork;
        //button.interactable = false;
        image.material.SetInt("Active", 0);
    }

    public void DestroyPerk()
    {
        DeactivePerk();
        Destroy(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.ActivateToolTip(perkData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.menuManager.menuCharacter.DeactivateToolTip();
    }

    public void DebugButton()
    {
        Activated = !Activated;
    }
    public override void NotifyUsed()
    {
        
    }
}
