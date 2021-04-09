using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthEffectsUI : MonoBehaviour
{
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

    public Transform EffectsAnchor;
    public Dictionary<EffectType, EffectDisplay> effectToDisplay = new Dictionary<EffectType, EffectDisplay>();
    public GameObject templateEffectDisplay;


    public void Start()
    {
        aAnchorHealthEffects = this.gameObject;
        SetupCamera();
        SetPositions();
    }

    public void SetupCamera()
    {
        canvas.worldCamera = WorldSystem.instance.cameraManager.mainCamera;
        canvas.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void SetPositions()
    {
        //Vector3 coordinates = WorldSystem.instance.cameraManager.mainCamera.WorldToScreenPoint(aAnchorHealthEffects.transform.localPosition);

        cAnchorHealthEffects.transform.position = aAnchorHealthEffects.transform.parent.position;
        cAnchorHealthEffects.transform.localPosition += new Vector3(0, -(aAnchorHealthEffects.transform.parent.GetComponent<CapsuleCollider>().height * 50 / 2), -100);

        if (intentDisplayAnchor != null)
        {
            intentDisplayAnchor.transform.position = aAnchorHealthEffects.transform.parent.position;
            intentDisplayAnchor.transform.localPosition += new Vector3(0, (aAnchorHealthEffects.transform.parent.GetComponent<CapsuleCollider>().height * 50 / 2), -100);
        }
    }

    public void UpdateHealthBar(int hitPoints, int maxHitPoints)
    {
        txtHealth.text = hitPoints.ToString() + "/" + maxHitPoints.ToString();
        sldHealth.value = 1.0f * hitPoints / maxHitPoints;
    }


    public IEnumerator UpdateShield(int shield)
    {
        objShield.SetActive(shield != 0);

        if (shield != 0)
        {
            sldShield.value = sldHealth.value;
            txtShield.text = shield.ToString();
        }
        yield return null;
    }

    public void SetEffect(EffectType effect, RuleEffect ruleEffect)
    {
        if (effectToDisplay.ContainsKey(effect))
        {
            if (ruleEffect.nrStacked == 0)
            {
                Destroy(effectToDisplay[effect].gameObject);
                effectToDisplay.Remove(effect);
            }
            else
                effectToDisplay[effect].SetLabel(ruleEffect);
        }
        else if (ruleEffect.nrStacked != 0)
        {
            GameObject effectObject = Instantiate(templateEffectDisplay, EffectsAnchor.position, Quaternion.Euler(0, 0, 0), EffectsAnchor) as GameObject;
            effectToDisplay[effect] = effectObject.GetComponent<EffectDisplay>();
            effectToDisplay[effect].SetSprite(WorldSystem.instance.uiManager.GetSpriteByName(ruleEffect.effectName));
            effectToDisplay[effect].SetLabel(ruleEffect);
        }

    }

}
