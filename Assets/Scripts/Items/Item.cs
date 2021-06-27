using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class Item : MonoBehaviour, IToolTipable, IEvents, IPointerEnterHandler
{
    Image image;
    Button button;
    bool _usable;
    public ItemCondition itemCondition;
    public ItemData itemData;
    public Transform tooltipAnchor;
    public TMP_Text counterText;
    public Effect effect;
    int _charges;
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

    void BindData()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        image.sprite = itemData.artwork;
        effect = Effect.GetEffect(gameObject, itemData.itemName, false);
        itemCondition = ItemCondition.GetItemCondition(itemData.itemCondition, this);
        charges = 1;
        Subscribe();
    }

    public void AddItem()
    {
        BindData();
    }

    public void RemoveItem()
    {
        Unsubscribe();
    }

    public void CheckItemUseCondition(WorldState state)
    {
        usable = (itemData.itemUseCondition.Contains(state) && charges > 0);
    }
    
    public void ButtonUseItem()
    {
        effect?.AddEffect();
        Debug.Log("Using Item!");
        itemCondition.UpdateCounter();
        charges--;
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string condition = ItemCondition.GetDescription(itemData.itemCondition);
        return (new List<string>{itemData.description, condition} , pos);
    }

    public void Subscribe()
    {
        EventManager.OnNewWorldStateEvent += CheckItemUseCondition;
    }

    public void Unsubscribe()
    {
        EventManager.OnNewWorldStateEvent -= CheckItemUseCondition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Over");
    }
}
