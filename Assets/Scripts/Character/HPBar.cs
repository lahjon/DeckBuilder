using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour, IEventSubscriber
{
    public Slider slider;
    public TMP_Text hpText;

    void Awake()
    {
        Subscribe();
    }

    public void UpdateHealth(int amount)
    {
        int current = WorldSystem.instance.characterManager.CurrentHealth;
        int max = WorldSystem.instance.characterManager.characterStats.GetStatValue(StatType.Health);
        if (max > 0)
        {
            slider.value = (float)current / (float)max;
            hpText.text = string.Format("{0}/{1}", current, max);
        }
    }

    public void Subscribe()
    {
        EventManager.OnHealthChangedEvent += UpdateHealth;
    }

    public void Unsubscribe()
    {
        EventManager.OnHealthChangedEvent -= UpdateHealth;
    }
}