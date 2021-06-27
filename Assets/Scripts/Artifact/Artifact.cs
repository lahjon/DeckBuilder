using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Artifact : MonoBehaviour, IToolTipable, IPointerEnterHandler, IPointerExitHandler
{
    public Rarity rarity;
    public string displayName;
    [TextArea(5,5)]
    public string tooltip;
    public RectTransform tooltipAnchor;
    Image image;
    public static Color highlighedColor = Color.white;
    public static Color normalColor = new Color(.8f, .8f, .8f, 1f);
    void Awake()
    {
        image = GetComponent<Image>();
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.ScreenToWorldPoint(tooltipAnchor.transform.position);
        pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        Debug.Log(tooltipAnchor.transform.position);
        Debug.Log(pos);
        Debug.Log(WorldSystem.instance.cameraManager.currentCamera);
        return (new List<string>{tooltip} , pos);
    }

    public void BindArtifactData(ArtifactData artifactData)
    {
        displayName = artifactData.artifactName;
        rarity = artifactData.rarity;
        tooltip = artifactData.description;
        GetComponent<Image>().sprite = artifactData.artwork;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlighedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }
}
