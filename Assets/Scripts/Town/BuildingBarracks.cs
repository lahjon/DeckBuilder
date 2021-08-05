using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarracks : Building
{
    public Sprite characterArtwork;
    public CharacterClassType characterClassType;
    public List<UseItemData> selectedItems;
    public List<UseItem> useItems = new List<UseItem>();
    public GameObject barracks, characterSelection;
    public List<GameObject> currentSelection = new List<GameObject>();
    
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        StepInto(barracks);
    }

    void ButtonEnterCharacterSelection()
    {
        currentSelection.Add(characterSelection);
        characterSelection.SetActive(true);
    }
    void ButtonStepBack()
    {
        characterSelection.SetActive(true);
    }

    void StepInto(GameObject room)
    {
        if (currentSelection.Count > 1)
        {
            
        }
        room.SetActive(true);
        currentSelection.Add(room);
    }

    void StepBack()
    {
        if (currentSelection.Count > 1)
        {
            currentSelection[currentSelection.Count].SetActive(false);
            currentSelection.RemoveAt(currentSelection.Count - 1);
            currentSelection[currentSelection.Count].SetActive(true);
        }
        else
        {
            CloseBuilding();
        }
    }

    void Start()
    {
        UpdateBarracks();
    }

    void UpdateBarracks()
    {
        characterArtwork = WorldSystem.instance.characterManager.character.characterData.artwork;
        characterClassType = WorldSystem.instance.characterManager.character.characterData.classType;
        selectedItems = WorldSystem.instance.useItemManager.equippedItems;

        useItems.ForEach(x => x.gameObject.SetActive(false));
        for (int i = 0; i < selectedItems.Count; i++)
        {
            useItems[i].gameObject.SetActive(true);
            useItems[i].itemData = selectedItems[i];
            useItems[i].BindData();
        }
    }
}
