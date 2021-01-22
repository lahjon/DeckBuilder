using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthEffects : MonoBehaviour
{
    public int maxHitPoints = 15;
    public int hitPoints = 15;
    private int shield = 10;

    public GameObject cAnchorHealthEffects;

    private GameObject aAnchorHealthEffects;
    public  Slider sldHealth;
    public TMP_Text txtHealth;
    public GameObject objShield;
    public Slider sldShield;
    public TMP_Text txtShield;

    public EffectDisplayManager effectDisplayManager;

    public Dictionary<EffectType, int> statusEffects = new Dictionary<EffectType, int>();

    public void Start()
    {
        aAnchorHealthEffects = this.gameObject;
        SetUIpositions();
        UpdateHealthBar();
        UpdateShieldUI();
    }


    public void UpdateHealthBar()
    {
        txtHealth.text = hitPoints.ToString() + "/" + maxHitPoints.ToString();
        sldHealth.value = 1.0f * hitPoints / maxHitPoints;
    }

    public void SetUIpositions()
    {
        Vector3 coordinates = WorldSystem.instance.cameraManager.currentCamera.WorldToScreenPoint(aAnchorHealthEffects.transform.position);
        cAnchorHealthEffects.transform.position = coordinates;
    }

    public void TakeDamage(int damage)
    {
        if (shield > 0)
        {
            int shieldDamage = Mathf.Min(shield, damage);
            RemoveBlock(shieldDamage);
            damage -= shieldDamage;
        }
        
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

        effectDisplayManager.SetEffect(effect.Type, statusEffects[effect.Type]);
    }

    public void EffectsStartTurn()
    {
        Debug.Log("Entered Effects decrement");
        List<EffectType> effects = new List<EffectType>(statusEffects.Keys);
        foreach (EffectType effect in effects)
        {
            Debug.Log("Change for effect " + effect);
            statusEffects[effect] += DatabaseSystem.instance.effectEndOfTurnBehavior[effect];
            effectDisplayManager.SetEffect(effect, statusEffects[effect]);
            if (statusEffects[effect] <= 0) statusEffects.Remove(effect);
        }
    }

    public void RecieveBlock(int x)
    {
        if (shield == 0 && x > 0)
            objShield.SetActive(true);

        shield += x;
        UpdateShieldUI();
    }


    private void UpdateShieldUI()
    {
        sldShield.value = sldHealth.value;
        txtShield.text = shield.ToString();
    }

    public void RemoveBlock(int x)
    {
        shield -= Mathf.Min(shield, x);
        if (shield == 0)
            DisableShield();
        else
            UpdateShieldUI();
    }

    public void RemoveAllBlock()
    {
        shield = 0;
        DisableShield();
    }

    private void DisableShield()
    {
        objShield.SetActive(false);
    }
}
