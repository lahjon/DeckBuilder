using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class TokenManager : Manager, ISaveableWorld, ISaveableTemp
{
    public List<TokenData> tokenDatas = new List<TokenData>();
    public List<Token> allTokens = new List<Token>();
    public List<int> selectedTokens = new List<int>();
    public List<int> unlockedTokens = new List<int>();
    public TokenMenu tokenMenu;
    public int tokenPoints;
    public int availableTokenPoints;
    public List<int> tokensRequireUpdate = new List<int>();
    int startingPoints = 1;

    protected override void Awake()
    {
        base.Awake();
        world.tokenManager = this;
    }
    protected override void Start()
    {
        base.Start(); 
        Init();
    }
    public void Init()
    { 
        for (int i = 0; i < tokenMenu.contentAll.transform.childCount; i++)
            BindTokenData(tokenMenu.contentAll.transform.GetChild(i).gameObject.GetComponent<Token>(), tokenDatas[i]);

        if (tokenPoints < startingPoints)
            tokenPoints = startingPoints;


        availableTokenPoints = tokenPoints;
        tokenMenu.Init();
    }
    void BindTokenData(Token token, TokenData tokenData)
    {
        token.artwork = tokenData.artwork;
        token.description = tokenData.description;
        token.cost = tokenData.cost;
        token.id = tokenData.itemId;
        token.gameObject.name = tokenData.name;

        if (tokenData.rarity == Rarity.Starting || unlockedTokens.Contains(token.id))
            token.unlocked = true;

        allTokens.Add(token);
        token.Init();
    }

    public void UnlockNewToken(int tokenId)
    {
        unlockedTokens.Add(tokenId);
        tokensRequireUpdate.Add(tokenId);
        GetTokenById(tokenId).unlocked = true;
        world.SaveProgression();
    }

    public Token GetTokenById(int tokenId)
    {
        return allTokens.Where(x => x.id == tokenId).FirstOrDefault();
    }

    public void AddTokenPoint()
    {
        tokenPoints++;
        availableTokenPoints++;
        tokenMenu.UpdatePoints();
        world.SaveProgression();
    }

    public void AddSelectedToken(Token token, bool init = false)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            token.itemEffect.AddItemEffect();
        }
        if (!init)
        {
            selectedTokens.Add(token.id);
        }
        tokenMenu.SelectToken(token); 
    }

    public void RemoveSelectedToken(Token token)
    {
        if (selectedTokens.Contains(token.id))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                token.itemEffect.RemoveItemEffect();
            }
            selectedTokens.Remove(token.id);
            tokenMenu.UnselectToken(token.id);
        }
    }
    
    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.selectedTokens = selectedTokens;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        selectedTokens = a_SaveData.selectedTokens;
    }

    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        unlockedTokens = a_SaveData.unlockedTokens;
        tokenPoints = a_SaveData.tokenPoints;
    }

    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedTokens = unlockedTokens;
        a_SaveData.tokenPoints = tokenPoints;
    }
}