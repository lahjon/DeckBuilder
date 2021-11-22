using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BuildingBarracks : Building
{
    public Image characterArtwork;
    public SelectableCharacter currentSelectedCharacter;
    public ProfessionType currentProfession;
    public List<Ability> selectedAbilities;
    public List<Ability> abilities = new List<Ability>();
    public GameObject barracks, characterSelection, professionSelection; // rooms
    List<SelectableCharacter> selectableCharacters = new List<SelectableCharacter>();
    public Transform characterParent;
    public ProfessionTooltip professionTooltip;
    public ProfessionType selectedProfessionType;
    public Profession profession1, profession2, profession3, selectedProfession;
    
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

    public void ButtonEnterProfessionSelection()
    {
        switch (WorldSystem.instance.characterManager.selectedCharacterClassType)
        {
            case CharacterClassType.Berserker:
                Debug.Log(DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == ProfessionType.Berserker1));
                profession1.BindData(DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == ProfessionType.Berserker1));
                profession2.BindData(DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == ProfessionType.Berserker2));
                profession3.BindData(DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == ProfessionType.Berserker3));
                break;
            default:
                return;
        }
        StepInto(professionSelection);
    }

    public void ButtonConfirmCharacterSelection()
    {
        characterArtwork.sprite = currentSelectedCharacter.GetComponent<Image>().sprite;
        UpdateCharacterManager();
        WorldSystem.instance.characterManager.characterVariablesUI.UpdateCharacterHUD();
        StepBack();
    }

    public void ButtonConfirmProfessionSelection()
    {
        WorldSystem.instance.characterManager.SwapProfession(selectedProfessionType);
        StepBack();
    }

    void UpdateCharacterManager()
    {
        WorldSystem.instance.characterManager.selectedCharacterClassType = currentSelectedCharacter.characterClassType;
        WorldSystem.instance.characterManager.SetupCharacterData(true);
        currentProfession = WorldSystem.instance.characterManager.profession;

        if (currentProfession == ProfessionType.Base)
        {
            switch (WorldSystem.instance.characterManager.selectedCharacterClassType)
            {
                case CharacterClassType.Berserker:
                    currentProfession = ProfessionType.Berserker1;
                    break;
                case CharacterClassType.Rogue:
                    currentProfession = ProfessionType.Rogue1;
                    break;
                case CharacterClassType.Splicer:
                    currentProfession = ProfessionType.Splicer1;
                    break;
                case CharacterClassType.Beastmaster:
                    currentProfession = ProfessionType.Beastmaster1;
                    break;
                
                default:
                    break;
            }
            
            WorldSystem.instance.characterManager.profession = currentProfession;
        }
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
        characterArtwork.sprite = WorldSystem.instance.characterManager.characterData.artwork;
        currentProfession = WorldSystem.instance.characterManager.profession;

        foreach (SelectableCharacter aCharacter in selectableCharacters)
        {
            if (!WorldSystem.instance.characterManager.unlockedCharacters.Contains(aCharacter.characterClassType)) 
                aCharacter.unlocked = false;
            else 
                aCharacter.unlocked = true;

            if (aCharacter.characterClassType == WorldSystem.instance.characterManager.characterData.classType)
                currentSelectedCharacter = aCharacter;
        }

        selectedAbilities = WorldSystem.instance.abilityManager.currentAbilities;
        selectedProfession.BindData(DatabaseSystem.instance.professionDatas.FirstOrDefault(x => x.professionType == WorldSystem.instance.characterManager.profession));

        abilities.ForEach(x => x.gameObject.SetActive(false));
        for (int i = 0; i < selectedAbilities.Count; i++)
        {
            Debug.Log(selectedAbilities[i]);
            abilities[i].gameObject.SetActive(true);
            abilities[i] = selectedAbilities[i];
            abilities[i].BindData(false);
        }
    }

    void UpdateProfessions()
    {

    }
}
