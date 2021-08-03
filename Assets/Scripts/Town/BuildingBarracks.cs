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
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
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
