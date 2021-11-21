using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Profession : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int professionIdx;
    public Button button;
    public Image image;
    public GameObject tooltip;
    public ProfessionData professionData;
    bool _unlocked;
    public bool Unlocked
    {
        get => _unlocked;
        set
        {
            _unlocked = value;
            if (_unlocked)
                button.interactable = true;
            else
                button.interactable = false;
        }
    }

    public void BindData(ProfessionData aProfessionData)
    {
        if (aProfessionData != null)
        {
            professionData = aProfessionData;
            image.sprite = professionData.artwork;
            if (WorldSystem.instance.characterManager.unlockedProfessions.Contains(aProfessionData.professionType))
                Unlocked = true;
            else
                Unlocked = false;
        }
    }

    public void ButtonSelectProfession()
    {
        if (professionData != null)
            WorldSystem.instance.townManager.barracks.selectedProfessionType = professionData.professionType;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.townManager.barracks.professionTooltip.DisableTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (professionData != null)
            WorldSystem.instance.townManager.barracks.professionTooltip.EnableTooltip(professionData);
    }
}
