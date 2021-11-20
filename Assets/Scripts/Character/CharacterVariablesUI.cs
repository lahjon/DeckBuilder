using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterVariablesUI : MonoBehaviour
{
    public TMP_Text healthValue, goldValue, classValue;
    public Image levelUpImage;
    public GameObject leftBar, topBar, currencyBar;
    Vector3 startPos;
    Vector3 offset = new Vector3(-0.4f,0,0);
    float moveSpeed = 0.2f;
    bool active;
    public GameObject content;
    Color positiveColor = new Color(0.5f, 0.5f, 0.5f);
    Color normalColor = new Color(0.0f, 0.0f, 0.0f);
    Color negativeColor = new Color(0.3f, 0.0f, 0.0f);
    public TMP_Text textStrength, textWit, textEnergy, textEndurance;
    public Image imageStrength, imageWit, imageEnergy, imageEndurance;

    void Start()
    {
        startPos = leftBar.transform.position;
        
        active = true;
        EventManager.OnNewWorldStateEvent += ToggleBar;
    }

    void ToggleBar(WorldState worldState)
    {
        if (worldState == WorldState.Overworld)
            topBar.SetActive(true);
        if (worldState == WorldState.Town)
            topBar.SetActive(false);
    }


    public void UpdateCharacterHUD()
    {
        if (WorldSystem.instance.characterManager != null)
        {
            int currentHealth = WorldSystem.instance.characterManager.currentHealth;
            int maxHealth = WorldSystem.instance.characterManager.characterStats.GetStat(StatType.Health);
            healthValue.text = currentHealth.ToString() + "/" + maxHealth.ToString();
            classValue.text = WorldSystem.instance.characterManager.selectedCharacterClassType.ToString();
            
            SetStats();
        }
    }

    void SetStats()
    { 
        SetStat(StatType.Endurance, imageEndurance, textEndurance);
        SetStat(StatType.Energy, imageEnergy, textEnergy);
        SetStat(StatType.Strength, imageStrength, textStrength);
        SetStat(StatType.Wit, imageWit, textWit);
    }

    void SetStat(StatType statType, Image statImage, TMP_Text statText)
    {
        CharacterStats stats = WorldSystem.instance.characterManager.characterStats;
        int value = stats.GetStat(statType);
        
        if (value == 0)
        {
            statText.text = "";
            statImage.color = normalColor;
        }
        else if(value < 0)
        {
            statText.text = "-" + value.ToString();
            statImage.color = negativeColor;
        }
        else
        {
            statText.text = "+" + value.ToString();
            statImage.color = positiveColor;
        }
    }

    public void ShowBar()
    {
        //Debug.Log("Show HUD");
        LeanTween.move(leftBar, startPos, moveSpeed).setEaseOutCubic().setOnComplete(() => active = true);
    }

    public void HideBar()
    {
        //Debug.Log("Hide HUD");
        active = false;
        LeanTween.move(leftBar, startPos + offset, moveSpeed).setEaseOutCubic();
    }

    public void ActivateLevelUp()
    {
        levelUpImage.gameObject.SetActive(true);
        LeanTween.scale(levelUpImage.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.5f).setEaseInBounce().setLoopPingPong();
    }

    public void DisableLevelUp()
    {
        levelUpImage.gameObject.SetActive(false);
    }

    public void ButtonDisplayDeck()
    {
        if (active)
        {
            if (WorldStateSystem.instance.currentOverlayState != OverlayState.Display)
            {
                WorldSystem.instance.deckDisplayManager.OpenDisplay();
            }
            else
            {
                WorldSystem.instance.deckDisplayManager.CloseDeckDisplay();
            }
        }
    }

    public void ButtonOpenCharacterSheet()
    {
        if (active)
        {
            WorldStateSystem.SetInCharacterSheet();
        }
    }
}
