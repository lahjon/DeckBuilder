using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum AbilityType
{
    None,
    Minor,
    Major
}
public class Ability : Item, IEventSubscriber, IToolTipable
{
    bool _usable;
    public ConditionCounting abilityCondition;
    public AbilityData abilityData;
    public TMP_Text counterText; 
    public int id;
    public Image image;
    public Button button;
    public Transform tooltipAnchor;
    bool initialized;
    int _charges;
    static float width = 100;
    public AbilityType abilityType;
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
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        initialized = true;   
        gameObject.SetActive(true);
    }

    public void BindData(AbilityData data)
    {
        if (data == null) return;
        if (initialized) RemoveAbility();
        else Initialize();

        abilityData = data;
        
        image.sprite = abilityData.artwork;
        itemEffect = ItemEffect.Factory(abilityData.itemEffectStruct, this);
        abilityCondition = new ConditionCounting(abilityData.itemCondition, OnPreconditionUpdate, OnConditionTrue);
        abilityCondition.Subscribe();
        charges = 1;
        Subscribe();
        
    }
    public void RemoveAbility()
    {
        abilityCondition.Unsubscribe();
        Unsubscribe();
    }

    public void CheckAbilityUseCondition(WorldState state)
    {
        usable = (abilityData.statesUsable.Contains(state) && charges > 0);
    }

    public (List<string> tips, Vector3 worldPosition, float offset) GetTipInfo()
    {
        Vector3 pos = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(tooltipAnchor.transform.position);
        string desc = string.Format("<b>" + abilityData.itemName + "</b>\n" + abilityData.description);
        string condition = abilityCondition.GetDescription(false);
        return (new List<string>{desc, condition} , pos, width);
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
        itemEffect?.ApplyEffect();
        Debug.Log("Using Ability!");
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
        Debug.Log("Used Item");
    }
}