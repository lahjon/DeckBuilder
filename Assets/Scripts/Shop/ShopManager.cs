using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Manager
{
    public ShopOverworld shop;
    public GameObject outOfStockPrefab;
    protected override void Awake()
    {
        base.Awake(); 
        world.shopManager = this;
    }
    public void LeaveShop()
    {
        Debug.Log("Leave Shop!");
        shop.gameObject.SetActive(false);
    }

    public void EnterShop()
    {
        Debug.Log("Enter Shop!");
        shop.gameObject.SetActive(true);
        shop.RestockShop();
    }
}
