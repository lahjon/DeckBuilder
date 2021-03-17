using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSheet : MonoBehaviour
{

    public TMP_Text strength, wisdom, endurance, cunning, speed, drawsize, health, handsize, damage, block, charClass, energy;
    public GameObject canvas;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            if(WorldStateSystem.instance.currentWorldState == WorldState.Combat || WorldStateSystem.instance.currentWorldState == WorldState.Shop || WorldStateSystem.instance.currentWorldState == WorldState.Overworld || WorldStateSystem.instance.currentWorldState == WorldState.Town)
            {
                ToggleCharacterSheet();
            }
        }
    }

    public void UpdateCharacterSheet()
    {
        CharacterManager characterManager = WorldSystem.instance.characterManager;
        

        // strength.text = characterManager.stats[CharacterStat.Strength].ToString();
        // wisdom.text = characterManager.stats[CharacterStat.Wisdom].ToString();
        // endurance.text = characterManager.stats[CharacterStat.Endurance].ToString();
        // cunning.text = characterManager.stats[CharacterStat.Cunning].ToString();
        // speed.text = characterManager.stats[CharacterStat.Speed].ToString();
        // drawsize.text = characterManager.cardDrawAmount.ToString();
        // health.text = characterManager.currentHealth.ToString() + "/" + characterManager.maxHealth.ToString();
        // handsize.text = characterManager.handSize.ToString();
        // damage.text = characterManager.damageModifier.ToString();
        // block.text = characterManager.blockModifier.ToString();
        charClass.text = characterManager.selectedCharacterClassType.ToString();
        energy.text = characterManager.character.energy.ToString();
    }

    public void ToggleCharacterSheet()
    {
        if(canvas.activeSelf)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
            UpdateCharacterSheet();
        }
    }
}
