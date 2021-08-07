using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UseItem : Item, IEvents
{
    bool _usable;
    public ItemCondition itemCondition;
    UseItemData _useItemData;
    public TMP_Text counterText; 
    public Effect effect; 
    int _charges;
    public new UseItemData itemData 
    {
        get => _useItemData;
        set 
        {
            _useItemData = value;
            BindData();
        }
    }
    public int charges
    { 
        get => _charges; 
        set
        {
            _charges = value;
            CheckItemUseCondition(WorldStateSystem.instance.currentWorldState);
        }
    }
    public bool usable
    {
        get => _usable; 
        set
        {
            _usable = value;
            button.interactable = _usable;  
        }
    }

    public override void BindData()
    {
        if (itemData == null) return;
        Initialize();
        
        image.sprite = itemData.artwork;
        effect = Effect.GetEffect(gameObject, itemData.name, false);
        itemCondition = ItemCondition.GetItemCondition(itemData.itemCondition, this);
        charges = 1;
        Subscribe();
    }

    public void AddItem()
    {
        
    }

    public void RemoveItem()
    {
        Unsubscribe();
    }

    public void CheckItemUseCondition(WorldState state)
    {
        usable = (itemData.itemUseCondition.Contains(state) && charges > 0);
    }

    public override (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>" + itemData.itemName + "</b>\n" + itemData.description);
        string condition = ItemCondition.GetDescription(itemData.itemCondition);
        return (new List<string>{desc, condition} , pos);
    }

    public void Subscribe()
    {
        EventManager.OnNewWorldStateEvent += CheckItemUseCondition;
    }

    public void Unsubscribe()
    {
        EventManager.OnNewWorldStateEvent -= CheckItemUseCondition;
    }
    public override void OnClick()
    {
        effect?.AddEffect();
        Debug.Log("Using Item!");
        itemCondition.UpdateCounter();
        charges--;
    }
}
