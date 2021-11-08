using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class TownInteractable : MonoBehaviour, IToolTipable
{
    [HideInInspector] public string buildingName;
    public GameEventStruct gameEvent;
    public BuildingType buildingType;
    public Transform tooltipAnchor;
    Image highlightImage;
    public Image artwork;
    Button button;
    Tween tween;
    static Color color = Color.black;
    [SerializeField] bool _highlighted;
    public bool Highlighted
    {
        get => _highlighted;
        set
        {
            _highlighted = value;
            if (_highlighted)
            {
                highlightImage.enabled = true;
                tween = highlightImage.DOColor(Color.blue, 1f).SetEase(Ease.OutSine).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                tween.Kill();
                highlightImage.color = color;
                highlightImage.enabled = false;
            }
        }
    }

    void Awake()
    {
        buildingName = buildingType.ToString();
        button = GetComponent<Button>();
        highlightImage = GetComponent<Image>();
        highlightImage.sprite = artwork.sprite;
        highlightImage.enabled = false;
    }
    public virtual void ButtonPress()
    {
        if (gameEvent.type != GameEventType.None)
        {
            WorldSystem.instance.gameEventManager.CreateEvent(gameEvent);
        }
        else
        {
            EnterBuilding();
        }
        WorldSystem.instance.toolTipManager.DisableTips();
    }

    void EnterBuilding()
    {
        if (WorldSystem.instance.townManager.buildings.FirstOrDefault(x => x.buildingType == buildingType) is BuildingStruct buildingStruct)
        {
            buildingStruct.building.EnterBuilding();
            Highlighted = false;
        }
    }

    public void DisableBuilding()
    {
        button.interactable = false;
    }

    public void EnableBuilding()
    {
        button.interactable = true;
    }

    public virtual (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        return (new List<string>{buildingName} , pos);
    }
}