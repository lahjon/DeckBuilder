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
        shop.gameObject.SetActive(false);
    }

    public void EnterShop()
    {
        shop.gameObject.SetActive(true);
        shop.RestockShop();
    }
}
