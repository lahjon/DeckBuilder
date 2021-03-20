using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthEffects : MonoBehaviour
{
    public int maxHitPoints = 15;
    public int hitPoints = 15;
    private int shield = 10;

    public GameObject cAnchorHealthEffects;
    public CombatActor combatActor;

    private GameObject aAnchorHealthEffects;
    public GameObject intentDisplayAnchor;
    public  Slider sldHealth;
    public TMP_Text txtHealth;
    public GameObject objShield;
    public Slider sldShield;
    public TMP_Text txtShield;
    public Canvas canvas;

    public EffectDisplayManager effectDisplayManager;

    public Dictionary<EffectType, RuleEffect> effectTypeToRule = new Dictionary<EffectType, RuleEffect>();

    public void Start()
    {
        aAnchorHealthEffects = this.gameObject;
        UpdateHealthBar();
        UpdateShieldUI();
        SetupCamera();
        SetUIpositions();
    }

    public void SetupCamera()
    {
        canvas.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvas.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void UpdateHealthBar()
    {
        txtHealth.text = hitPoints.ToString() + "/" + maxHitPoints.ToString();
        sldHealth.value = 1.0f * hitPoints / maxHitPoints;
    }

    public void SetUIpositions()
    {
        Vector3 coordinates = WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(aAnchorHealthEffects.transform.localPosition);

        cAnchorHealthEffects.transform.position = aAnchorHealthEffects.transform.parent.position;
        cAnchorHealthEffects.transform.localPosition += new Vector3(0,-(aAnchorHealthEffects.transform.parent.GetComponent<CapsuleCollider>().height*50 / 2),-100);

        if(intentDisplayAnchor != null)
        {
            intentDisplayAnchor.transform.position =  aAnchorHealthEffects.transform.parent.position;
            intentDisplayAnchor.transform.localPosition += new Vector3(0,(aAnchorHealthEffects.transform.parent.GetComponent<CapsuleCollider>().height*50 / 2),-100);
        }
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
            WorldSystem.instance.combatManager.combatController.ReportDeath(combatActor);
    }

    public void RecieveEffectNonDamageNonBlock(CardEffect effect)
    {
        if (effectTypeToRule.ContainsKey(effect.Type)) {
            effectTypeToRule[effect.Type].nrStacked += effect.Value * effect.Times;
            effectDisplayManager.SetEffect(effect.Type, effectTypeToRule[effect.Type]);
            return;
        }

        effectTypeToRule[effect.Type] = effect.Type.GetRuleEffect();
        effectTypeToRule[effect.Type].healthEffects = this;
        effectTypeToRule[effect.Type].AddFunctionToRules();

        effectTypeToRule[effect.Type].healthEffects = this;
        effectTypeToRule[effect.Type].nrStacked = effect.Value * effect.Times;
        Debug.Log("Got effect: " + effect.Type.ToString() + ", " + (effect.Value * effect.Times).ToString());
        effectDisplayManager.SetEffect(effect.Type, effectTypeToRule[effect.Type]);
    }

    public void EffectsOnNewTurnBehavior()
    {
        List<EffectType> effects = new List<EffectType>(effectTypeToRule.Keys);
        foreach (EffectType effect in effects)
        {
            effectTypeToRule[effect].OnNewTurnBehaviour();
            effectDisplayManager.SetEffect(effect, effectTypeToRule[effect]);
            if (effectTypeToRule[effect].nrStacked <= 0)
            {
                effectTypeToRule[effect].RemoveFunctionFromRules();
                effectTypeToRule.Remove(effect);
            }
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
