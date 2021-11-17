using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Ability : MonoBehaviour, IEventSubscriber, IToolTipable
{
    bool _usable;
    public ConditionCounting abilityCondition;
    AbilityData _abilityData;
    public TMP_Text counterText; 
    public ItemEffect itemEffect; 
    public int id;
    public Image image;
    public Button button;
    public Transform tooltipAnchor;
    bool initialized;
    int _charges;
    public AbilityData abilityData 
    {
        get => _abilityData;
        set 
        {
            _abilityData = value;
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
    void Initialize()
    {
        if (!initialized)
        {
            image = GetComponent<Image>();
            button = GetComponent<Button>();
            initialized = true;   
        }
    }

    public void BindData(bool allData = true)
    {
        if (abilityData == null) return;
        Initialize();
        
        image.sprite = abilityData.artwork;
        if (allData)
        {
            itemEffect = ItemEffectManager.CreateItemEffect(abilityData.itemEffectStruct, abilityData.itemName);
            abilityCondition = new ConditionCounting(abilityData.itemCondition, OnPreconditionUpdate, OnConditionTrue);
            abilityCondition.Subscribe();
            charges = 1;
            Subscribe();
        }
    }
    public void RemoveItem()
    {
        WorldSystem.instance.abilityManager.usedAbilitySlots--;
        abilityCondition.Unsubscribe();
        Unsubscribe();
        Destroy(gameObject);
    }

    public void ConditionPass()
    {

    }

    public void CheckItemUseCondition(WorldState state)
    {
        usable = (abilityData.statesUsable.Contains(state) && charges > 0);
    }

    public (List<string> tips, Vector3 worldPosition) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>" + abilityData.itemName + "</b>\n" + abilityData.description);
        string condition = abilityCondition.GetDescription(false);
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
    public void OnClick()
    {
        itemEffect?.AddItemEffect();
        Debug.Log("Using Item!");
        counterText.text = (abilityCondition.requiredAmount - abilityCondition.currentAmount).ToString();
        charges--;
    }

    public void OnPreconditionUpdate()
    {
        Debug.Log("Dick");
        counterText.text = (abilityCondition.requiredAmount - abilityCondition.currentAmount).ToString();
    }

    public void OnConditionTrue()
    {
        Debug.Log("Dick2");
        counterText.text = "";
        abilityCondition.currentAmount = 0;
        charges++;
    }
}