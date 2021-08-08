using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuildingBarracks : Building
{
    public Image characterArtwork;
    public SelectableCharacter currentSelectedCharacter;
    public Profession currentProfession;
    public List<UseItemData> selectedItems;
    public List<UseItem> useItems = new List<UseItem>();
    public GameObject barracks, characterSelection; // rooms
    List<SelectableCharacter> selectableCharacters = new List<SelectableCharacter>();
    public Transform characterParent;
    
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        UpdateBarracks();
        StepInto(barracks);
    }

    public void ButtonEnterCharacterSelection()
    {
        StepInto(characterSelection);
        currentSelectedCharacter.SelectCharacter();
    }

    public void ButtonConfirmCharacterSelection()
    {
        characterArtwork.sprite = currentSelectedCharacter.GetComponent<Image>().sprite;
        UpdateCharacterManager();
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        StepBack();
    }

    void UpdateCharacterManager()
    {
        WorldSystem.instance.characterManager.selectedCharacterClassType = currentSelectedCharacter.characterClassType;
        WorldSystem.instance.characterManager.SetupCharacterData(true);
        currentProfession = WorldSystem.instance.characterManager.character.profession;

        // TODO: uncomment when we have professions integrated in DB

        // if (currentProfession == Profession.Base)
        // {
        //     switch (WorldSystem.instance.characterManager.selectedCharacterClassType)
        //     {
        //         case CharacterClassType.Berserker:
        //             currentProfession = Profession.Berserker1;
        //             break;
        //         case CharacterClassType.Rogue:
        //             currentProfession = Profession.Rogue1;
        //             break;
        //         case CharacterClassType.Splicer:
        //             currentProfession = Profession.Splicer1;
        //             break;
        //         case CharacterClassType.Beastmaster:
        //             currentProfession = Profession.Beastmaster1;
        //             break;
                
        //         default:
        //             break;
        //     }
            
        //     WorldSystem.instance.characterManager.character.profession = currentProfession;
        // }

        Debug.Log("Update selected character");
    }

    void Start()
    {
        UpdateBarracks();
        for (int i = 0; i < characterParent.childCount; i++)
        {
            selectableCharacters.Add(characterParent.GetChild(i).GetComponent<SelectableCharacter>());
        }
        SelectableCharacter.buildingBarracks = this;
    }

    public void SelectCharacter(CharacterClassType classType)
    {
        currentSelectedCharacter.DeselectCharacter();

        if (selectableCharacters.FirstOrDefault(x => x.characterClassType == classType) is SelectableCharacter character)
        {
            currentSelectedCharacter = character;
            character.SelectCharacter();
        }
    }

    void UpdateBarracks()
    {
        characterArtwork.sprite = WorldSystem.instance.characterManager.character.characterData.artwork;
        currentProfession = WorldSystem.instance.characterManager.character.profession;

        foreach (SelectableCharacter aCharacter in selectableCharacters)
        {
            if (!WorldSystem.instance.characterManager.unlockedCharacters.Contains(aCharacter.characterClassType)) 
                aCharacter.unlocked = false;
            else 
                aCharacter.unlocked = true;

            if (aCharacter.characterClassType == WorldSystem.instance.characterManager.character.characterData.classType)
                currentSelectedCharacter = aCharacter;
        }

        selectedItems = WorldSystem.instance.useItemManager.equippedItems;

        useItems.ForEach(x => x.gameObject.SetActive(false));
        for (int i = 0; i < selectedItems.Count; i++)
        {
            Debug.Log(selectedItems[i]);
            useItems[i].gameObject.SetActive(true);
            useItems[i].itemData = selectedItems[i];
            useItems[i].BindData();
        }
    }
}
