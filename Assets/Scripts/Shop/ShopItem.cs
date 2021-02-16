using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public int shardCost;
    public string itemName;
    public TMP_Text textName;
    public TMP_Text textCost;
    public BuildingType building;

    private TownManager townManager;
    private CharacterManager characterManager;
    

    void Start()
    {
        townManager = WorldSystem.instance.townManager;
        characterManager = WorldSystem.instance.characterManager;
        textCost.text = shardCost.ToString();
        textName.text = itemName;
    }

    public void BuyItem()
    {
        if (shardCost <= characterManager.shard && townManager.UnlockBuilding(building))
        {
            characterManager.shard -= shardCost;

            Transform parent = gameObject.transform.parent;
            
            DestroyImmediate(gameObject);
            GameObject outOfStock = Instantiate(WorldSystem.instance.shopManager.outOfStockPrefab);
            outOfStock.transform.SetParent(parent);
    }
        else
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough shards!");
        }
    }
}
