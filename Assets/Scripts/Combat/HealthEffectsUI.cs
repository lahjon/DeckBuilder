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

    public Transform EffectsAnchorOath;
    public Transform EffectsAnchorOther;
    public GameObject templateEffectDisplay;
    public GameObject templateNotification;

    public Queue<HealtUINotification> uINotificators = new Queue<HealtUINotification>();
    public Transform AnchorEffectNotifications;
    public Transform AnchorHealthNotifications;

    private Color32 lifeRed = new Color32(255, 0, 0, 255);
    private Color32 generealBlack = new Color32(0, 0, 0, 255);
    private Color32 generealWhite = new Color32(255, 255, 255, 255);
    public Image fillArea;

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

    public EffectDisplay GetEffectDisplay(CardEffect effect) 
    {
        Transform parent = effect.type.isOath ? EffectsAnchorOath : EffectsAnchorOther;
        parent.gameObject.SetActive(true);
        EffectDisplay display = Instantiate(templateEffectDisplay, parent).GetComponent<EffectDisplay>();
        display.SetBackingEffect(effect);
        StartNotificationEffect(effect.type.effectType.ToString());
        return display;
    }

    public void RetireEffectDisplay(EffectDisplay display)
    {
        display.CancelAnimation();
        Destroy(display.gameObject);
        Transform parent = display.backingEffect.type.isOath ? EffectsAnchorOath : EffectsAnchorOther;
        if (parent.childCount == 0)
        {
            StartNotificationEffect(string.Format("{0} wore off", display.backingEffect.type.effectType));
            parent.gameObject.SetActive(false);
        }
    }

    public void StartNotificationEffect(string notification)
    {
        HealtUINotification noti = GetNotification();
        noti.Kinetor = noti.DecreasingRise;
        noti.ResetLabel(notification, generealBlack, AnchorEffectNotifications);
    }

    public void StartNotificationLifeLoss(int lifeChange)
    {
        HealtUINotification noti = GetNotification();
        noti.Kinetor = noti.Poly2Rise;
        noti.ResetLabel(lifeChange.ToString(), lifeRed, AnchorHealthNotifications,1.5f);
    }

    public void StartNotificationCardName(string notification)
    {
        Debug.Log("Starting cardname print");
        HealtUINotification noti = GetNotification();
        noti.Kinetor = noti.DecreasingRise;
        noti.ResetLabel(notification, generealWhite, AnchorEffectNotifications);
        noti.label.fontSize = 1200;
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
