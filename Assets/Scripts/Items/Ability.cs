using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Ability : Item, IEventSubscriber, IToolTipable
{
    bool _usable;
    public ConditionCounting abilityCondition;
    AbilityData _abilityData;
    public TMP_Text counterText; 
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
            CheckAbilityUseCondition(WorldStateSystem.instance.currentWorldState);
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
            itemEffect = WorldSystem.instance.itemEffectManager.CreateItemEffect(abilityData.itemEffectStruct, this, abilityData.itemName);
            abilityCondition = new ConditionCounting(abilityData.itemCondition, OnPreconditionUpdate, OnConditionTrue);
            abilityCondition.Subscribe();
            charges = 1;
            Subscribe();
        }
    }
    public void RemoveAbility()
    {
        WorldSystem.instance.abilityManager.usedAbilitySlots--;
        abilityCondition.Unsubscribe();
        Unsubscribe();
        Destroy(gameObject);
    }

    public void ConditionPass()
    {

    }

    public void CheckAbilityUseCondition(WorldState state)
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
        EventManager.OnNewWorldStateEvent += CheckAbilityUseCondition;
    }

    public void Unsubscribe()
    {
        EventManager.OnNewWorldStateEvent -= CheckAbilityUseCondition;
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
        counterText.text = (abilityCondition.requiredAmount - abilityCondition.currentAmount).ToString();
    }

    public void OnConditionTrue()
    {
        counterText.text = "";
        abilityCondition.currentAmount = 0;
        charges++;
    }
    public override void NotifyUsed()
    {
        throw new System.NotImplementedException();
    }
}