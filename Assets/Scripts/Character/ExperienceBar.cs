using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExperienceBar : MonoBehaviour, IEventSubscriber
{
    public Slider slider;
    public TMP_Text xpText;
    public Image fillImage;
    Tween tween;
    Color normal;
    Color flashing = new Color(.91f, .61f, .80f);
    public TMP_Text levelText;

    void Awake()
    {
        Subscribe();
        normal = fillImage.color;
    }

    public void UpdateExperience(int amount)
    {
        int current = WorldSystem.instance.levelManager.CurrentExperience;
        int max = WorldSystem.instance.levelManager.requiredExperience;
        if (max > 0)
        {
            slider.value = (float)current / (float)max;
            xpText.text = string.Format("{0}/{1}", current, max);
        }
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

    public void Flash()
    {
        if (tween == null)
            tween = fillImage.DOColor(flashing, .5f).SetLoops(-1, LoopType.Yoyo).OnComplete(() => fillImage.color = normal).OnKill(() => fillImage.color = normal);
    }
    public void StopFlash()
    {
        tween?.Kill();
        tween = null;
    }

    public void Subscribe()
    {
        EventManager.OnExperienceChangedEvent += UpdateExperience;
        EventManager.OnLevelUpEvent += UpdateLevel;
    }

    public void Unsubscribe()
    {
        EventManager.OnExperienceChangedEvent -= UpdateExperience;
        EventManager.OnLevelUpEvent -= UpdateLevel;
    }
}