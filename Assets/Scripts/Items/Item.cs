using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class Item : MonoBehaviour, IToolTipable
{
    public int id;
    protected Image image;
    public Button button;
    protected ItemData _itemData;
    public Transform tooltipAnchor;
    bool initialized;
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
        Initialize();
    }

    protected void Initialize()
    {
        if (!initialized)
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            initialized = true;   
        }
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
    public virtual void BindData(bool allData = true)
    {   
        Initialize();
        if (itemData != null)
        {
            image.sprite = itemData.artwork;
            id = itemData.itemId;
        }
    }
}
