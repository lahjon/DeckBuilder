using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class HealthEffectsUI : MonoBehaviour
{
    public GameObject cAnchorHealthEffects;
    public CombatActor combatActor;
    private LTDescr healthAction;

    private GameObject aAnchorHealthEffects;
    public GameObject intentDisplayAnchor;
    public  Slider sldHealth;
    public  Slider sldHealthTemp;
    public float sldHealthValue;
    public TMP_Text txtHealth;
    public GameObject objShield;
    public GameObject shieldIcon;
    public Slider sldShield;
    public TMP_Text txtShield;
    public Canvas canvas;
    IEnumerator Decay;

    public Transform EffectsAnchor;
    public Dictionary<EffectType, EffectDisplay> effectToDisplay = new Dictionary<EffectType, EffectDisplay>();
    public GameObject templateEffectDisplay;
    public GameObject templateNotification;

    public Queue<HealtUINotification> uINotificators = new Queue<HealtUINotification>();
    public Transform AnchorEffectNotifications;
    public Transform AnchorHealthNotifications;


    private Queue<RuleEffect> queuedEffectsAnimation = new Queue<RuleEffect>();
    private IEnumerator coroutineEffectAdder;

    private Color32 lifeRed = new Color32(255, 0, 0, 255);
    private Color32 generealWhite = new Color32(0, 0, 0, 255);

    public void Start()
    {
        aAnchorHealthEffects = this.gameObject;
        sldHealthTemp.value = sldHealth.value;
        SetupCamera();
        SetPosition();
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetupCamera()
    {
        canvas.worldCamera = WorldSystem.instance.cameraManager.combatCamera;
        canvas.planeDistance = WorldSystem.instance.uiManager.planeDistance;
    }

    public void SetPosition()
    {
        cAnchorHealthEffects.transform.position = aAnchorHealthEffects.transform.parent.position;
    }

    public void UpdateHealthBar(int hitPoints, int maxHitPoints)
    {
        txtHealth.text = hitPoints.ToString() + "/" + maxHitPoints.ToString();
        float toValue = 1.0f * hitPoints / maxHitPoints;
        sldHealth.value = toValue;

        DOTween.To(() => sldHealthTemp.value, x => sldHealthTemp.value = x, toValue, 1.0f).SetEase(Ease.InExpo);
    }

    public IEnumerator UpdateShield(int shield)
    {
        objShield.SetActive(shield != 0);

        shieldIcon.transform.localScale = Vector3.one;

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

        if(effect.nrStacked == 0 && effectToDisplay.ContainsKey(effectType))
        {
            if (effectToDisplay[effectType] is EffectDisplay tempEffect)
            {
                tempEffect.CancelAnimation();
            }
            Destroy(effectToDisplay[effectType].gameObject);
            effectToDisplay.Remove(effectType);
            StartEffectNotification(effect.effectName + " wore off");
            return;
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
            effectToDisplay[effectType].backingType = effect.type;
            StartEffectNotification(effect.effectName);
        }

    }

    public void StartEffectNotification(string notification)
    {
        HealtUINotification noti = GetNotification();
        noti.Kinetor = noti.DecreasingRise;
        noti.ResetLabel(notification, generealWhite, AnchorEffectNotifications);
    }

    public void StartLifeLossNotification(int lifeChange)
    {
        HealtUINotification noti = GetNotification();
        noti.Kinetor = noti.Poly2Rise;
        noti.ResetLabel(lifeChange.ToString(), lifeRed, AnchorHealthNotifications,1.5f);
    }

    private HealtUINotification GetNotification()
    {
        if (uINotificators.Count == 0)
        {
            GameObject notObject = Instantiate(templateNotification, AnchorHealthNotifications.position, Quaternion.Euler(0, 0, 0), AnchorHealthNotifications);
            HealtUINotification noti = notObject.GetComponent<HealtUINotification>();
            noti.healthEffectsUI = this;
            return noti;
        }
        else
            return uINotificators.Dequeue();
    }

}
