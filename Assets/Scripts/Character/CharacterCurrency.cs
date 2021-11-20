using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCurrency : MonoBehaviour, ISaveableCharacter, ISaveableWorld, ISaveableTemp
{
    int _shard, _gold, _fullEmber, _ember, _armorShard ;
    public int fullEmber 
    {
        get
        {
            return _fullEmber;
        }
        set
        {
            _fullEmber = value;
            EventManager.CurrencyChanged(CurrencyType.FullEmber, value);
        }
    }
    public int shard 
    {
        get
        {
            return _shard;
        }
        set
        {
            _shard = value;
            EventManager.CurrencyChanged(CurrencyType.Shard, value);
        }
    }
    public int gold 
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            EventManager.CurrencyChanged(CurrencyType.Gold, value);
        }
    }
    public int armorShard 
    {
        get
        {
            return _armorShard;
        }
        set
        {
            _armorShard = value;
            EventManager.CurrencyChanged(CurrencyType.ArmorShard, value);
        }
    }
    public int ember 
    {
        get
        {
            return _ember;
        }
        set
        {
            _ember = value;
            EventManager.CurrencyChanged(CurrencyType.Ember, value);
        }
    }

    public void LoadFromSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        //Debug.Log("asd");
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        _gold = a_SaveData.gold;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        _shard = a_SaveData.shard;
    }

    public void PopulateSaveDataCharacter(SaveDataCharacter a_SaveData)
    {
        //Debug.Log("asd");
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.gold = gold;
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.shard = _shard;
    }
}
