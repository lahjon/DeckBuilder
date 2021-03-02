using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class StatsController : MonoBehaviour
{
    public TMP_Text hpValue;
    public TMP_Text energyValue;
    public TMP_Text drawCardValue;
    public TMP_Text levelValue;
    public TMP_Text blockValue;
    public TMP_Text damageValue;

    public TMP_Text hpValueMod;
    public TMP_Text energyValueMod;
    public TMP_Text drawCardValueMod;
    public TMP_Text blockValueMod;
    public TMP_Text damageValueMod;

    public CharacterCreator characterCreator;

    public void UpdateStats()
    {
        hpValue.text = characterCreator.selectedCharacter.characterData.maxHp.ToString();
        energyValue.text = characterCreator.selectedCharacter.characterData.energy.ToString();
        drawCardValue.text = characterCreator.selectedCharacter.characterData.drawCardsAmount.ToString();
        levelValue.text = characterCreator.selectedCharacter.characterData.level.ToString();
        blockValue.text = characterCreator.selectedCharacter.characterData.blockModifier.ToString();
        damageValue.text = characterCreator.selectedCharacter.characterData.damageModifier.ToString();

        hpValueMod.text = (characterCreator.selectedCharacter.maxHp - characterCreator.selectedCharacter.characterData.maxHp).ToString();
        energyValueMod.text = (characterCreator.selectedCharacter.energy - characterCreator.selectedCharacter.characterData.energy).ToString();
        drawCardValueMod.text = (characterCreator.selectedCharacter.drawCardsAmount - characterCreator.selectedCharacter.characterData.drawCardsAmount).ToString();
        blockValueMod.text = (characterCreator.selectedCharacter.blockModifier - characterCreator.selectedCharacter.characterData.blockModifier).ToString();
        damageValueMod.text = (characterCreator.selectedCharacter.damageModifier - characterCreator.selectedCharacter.characterData.damageModifier).ToString();
    }
}
