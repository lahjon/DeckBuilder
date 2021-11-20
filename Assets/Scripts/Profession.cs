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
    public ProfessionType professionType;

    void Start()
    {
        
    }

    public void ButtonSwapProfession()
    {
        WorldSystem.instance.characterManager.SwapProfession(professionType);
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        WorldSystem.instance.townManager.barracks.professionTooltip.DisableTooltip();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ASd");
        WorldSystem.instance.townManager.barracks.professionTooltip.EnableTooltip(professionType);
    }
}
