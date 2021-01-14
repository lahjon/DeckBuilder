using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActorDummy : MonoBehaviour
{
    public int maxHitPoints = 15;
    public int hitPoints = 15;

    public GameObject cAnchorHealthEffects;
    public GameObject aAnchorHealthEffects;
    public PlayerController cardManager;
    
    private Slider sldHealth;
    private TMP_Text txtHealth;
    public TMP_Text txtEffects;

    public Dictionary<EffectType, int> statusEffects = new Dictionary<EffectType, int>();
    public void Awake()
    {
        sldHealth = GetComponentInChildren<Slider>();
        txtHealth = GetComponentInChildren<TMP_Text>();
    }

    public void Start()
    {
        SetUIpositions();
        UpdateHealthBar();
        UpdateEffectsDisplay();
    }

    public void OnMouseOver()
    {
        if(cardManager.ActiveEnemy is null) cardManager.ActiveEnemy = this;
    }

    public void OnMouseExit()
    {
        if (cardManager.ActiveEnemy == this) cardManager.ActiveEnemy = null;
    }

    public void OnMouseDown()
    {
        cardManager.EnemyClicked(this);
    }

    public void UpdateHealthBar()
    {
        txtHealth.text = hitPoints.ToString() + "/" + maxHitPoints.ToString();
        sldHealth.value = 1.0f * hitPoints / maxHitPoints;
    }

    public void SetUIpositions()
    {
        Debug.Log(Camera.main.name);
        Vector3 coordinates = Camera.main.WorldToScreenPoint(aAnchorHealthEffects.transform.position);
        cAnchorHealthEffects.transform.position = coordinates;
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= Mathf.Min(hitPoints, damage);
        UpdateHealthBar();
        if (hitPoints == 0)
            Destroy(gameObject);
    }

    public void RecieveEffect(CardEffect effect)
    {
        if (statusEffects.ContainsKey(effect.Type))
            statusEffects[effect.Type] += effect.Value;
        else
            statusEffects[effect.Type] = effect.Value;
         
        UpdateEffectsDisplay(); //CHANGE TO UPDATING JUST THAT ICON WHEN IT IS ICONS!!!
    }

    public void EffectsStartTurn()
    {
        Debug.Log("Entered Effects decrement");
        List<EffectType> effects = new List<EffectType>();
        effects.AddRange(statusEffects.Keys);
        foreach (EffectType effect in effects)
        {
            Debug.Log("Change for effect " + effect);
            statusEffects[effect] += DatabaseSystem.instance.effectEndOfTurnBehavior[effect];
            if (statusEffects[effect] <= 0) statusEffects.Remove(effect);
        }

        UpdateEffectsDisplay();
    }

    public void UpdateEffectsDisplay()
    {
        txtEffects.text = "";
        foreach (EffectType effect in statusEffects.Keys)
            txtEffects.text += effect.ToString() + ": " + statusEffects[effect].ToString() + "\n";
    }
}
