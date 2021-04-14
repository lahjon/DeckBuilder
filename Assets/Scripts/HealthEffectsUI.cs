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
    
    public List<HealtUINotification> uINotificators;
    public Transform AnchorEffectNotifications;
    public Transform AnchorHealthNotifications;


    private Queue<RuleEffect> queuedEffectsAnimation = new Queue<RuleEffect>();
    private IEnumerator coroutineEffectAdder;

    private Color32 lifeRed = new Color32(255, 0, 0, 255);
    private Color32 generealWhite = new Color32(0, 0, 0, 255);

    public void Start()
    {
        aAnchorHealthEffects = this.gameObject;
        SetupCamera();
        SetPositions();
    }

    public void OnDisable()
    {
        StopAllCoroutines();
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


    public void ModifyEffectUI(RuleEffect effect)
    {
        queuedEffectsAnimation.Enqueue(effect);
        if(coroutineEffectAdder is null)
        {
            coroutineEffectAdder = EffectDequeuer();
            if(gameObject.activeSelf) StartCoroutine(coroutineEffectAdder);
        }
    }

    public IEnumerator EffectDequeuer()
    {
        while(queuedEffectsAnimation.Count != 0)
        {
            RuleEffect current = queuedEffectsAnimation.Dequeue();
            UpdateEffectUI(current);
            yield return new WaitForSeconds(0.3f);
        }

        coroutineEffectAdder = null;
        yield return null;
    }

    public void UpdateEffectUI(RuleEffect effect)
    {
        EffectType effectType = effect.type;

        if(effect.nrStacked == 0)
        {
            if (effectToDisplay.ContainsKey(effectType)){
                Destroy(effectToDisplay[effectType].gameObject);
                effectToDisplay.Remove(effectType);
                StartEffectNotification(effect.effectName + " wore off");
                return;
            }
        }

        if (effectToDisplay.ContainsKey(effectType))
        {
            effectToDisplay[effectType].SetLabel(effect.strStacked());
        }
        else if (effect.nrStacked != 0)
        {
            GameObject effectObject = Instantiate(templateEffectDisplay, EffectsAnchor.position, Quaternion.Euler(0, 0, 0), EffectsAnchor);
            effectToDisplay[effectType] = effectObject.GetComponent<EffectDisplay>();
            effectToDisplay[effectType].SetSprite(WorldSystem.instance.uiManager.GetSpriteByName(effect.effectName));
            effectToDisplay[effectType].SetLabel(effect.strStacked());
            StartEffectNotification(effect.effectName);
        }

    }

    public void StartEffectNotification(string notification)
    {
        for(int i = 0; i < uINotificators.Count; i++)
        {
            if (!uINotificators[i].isActiveAndEnabled)
            {
                uINotificators[i].transform.SetParent(AnchorEffectNotifications);
                uINotificators[i].SetColor(generealWhite);
                uINotificators[i].gameObject.SetActive(true);
                uINotificators[i].SetKineticFun("LinearRise");
                uINotificators[i].ResetLabel(notification);
                return;
            }
        }
    }

    public void StartLifeLossNotification(int lifeChange)
    {
        for (int i = 0; i < uINotificators.Count; i++)
        {
            if (!uINotificators[i].isActiveAndEnabled)
            {
                uINotificators[i].transform.SetParent(AnchorHealthNotifications);
                uINotificators[i].gameObject.SetActive(true);
                uINotificators[i].SetColor(lifeRed);
                uINotificators[i].SetKineticFun("Poly2Rise");
                uINotificators[i].ResetLabel(lifeChange.ToString(),1.5f);
                return;
            }
        }
    }

    

}
