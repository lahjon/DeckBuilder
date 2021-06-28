using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class Item : MonoBehaviour, IToolTipable
{
    protected Image image;
    protected Button button;
    protected ItemData _itemData;
    public Transform tooltipAnchor;
    public virtual ItemData itemData
    {
        get => _itemData;
        set
        {
            _itemData = value;
            BindData();
        }
    }
    protected void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }
    protected void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    public virtual (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>" + itemData.itemName + "</b>\n" + itemData.description);
        return (new List<string>{desc} , pos);
    }
    public abstract void OnClick();
    public virtual void BindData()
    {
        image.sprite = itemData.artwork;
    }
}
