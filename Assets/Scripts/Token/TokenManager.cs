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
    public List<GameObject> allTokens = new List<GameObject>();
    public List<string> selectedTokens = new List<string>();
    public List<string> unlockedTokens = new List<string>();
    public TokenMenu tokenMenu;
    public int tokenPoints;
    public int availableTokenPoints;
    public List<string> tokensRequireUpdate = new List<string>();
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
        {
            BindTokenData(tokenMenu.contentAll.transform.GetChild(i).gameObject, tokenDatas[i]);
        }

        if (tokenPoints < startingPoints)
        {
            tokenPoints = startingPoints;
        }

        availableTokenPoints = tokenPoints;
        
        tokenMenu.Init();
    }
    void BindTokenData(GameObject aToken, TokenData tokenData)
    {
        Token token = aToken.GetComponent<Token>();
        token.artwork = tokenData.artwork;
        token.description = tokenData.description;
        token.cost = tokenData.cost;
        aToken.name = tokenData.name;

        if (tokenData.rarity == Rarity.Starting || unlockedTokens.Contains(aToken.name))
        {
            token.unlocked = true;
        }

        Effect.GetEffect(aToken, tokenData.name);

        allTokens.Add(aToken);
        token.Init();
    }

    public void UnlockNewToken(string tokenName)
    {
        unlockedTokens.Add(tokenName);
        tokensRequireUpdate.Add(tokenName);
        GetTokenByName(tokenName, allTokens).GetComponent<Token>().unlocked = true;
        world.SaveProgression();
    }

    public GameObject GetTokenByName(string tokenName, List<GameObject> tokenList)
    {
        return tokenList.Where(x => x.name == tokenName).FirstOrDefault();
    }

    public void AddTokenPoint()
    {
        tokenPoints++;
        availableTokenPoints++;
        tokenMenu.UpdatePoints();
        world.SaveProgression();
    }

    public void AddSelectedToken(GameObject token, bool init = false)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            token.GetComponent<Effect>().AddEffect();
        }
        if (!init)
        {
            selectedTokens.Add(token.name);
        }
        tokenMenu.SelectToken(token); 
    }

    public void RemoveSelectedToken(GameObject token)
    {
        if (selectedTokens.Contains(token.name))
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                token.GetComponent<Effect>().RemoveEffect();
            }
            selectedTokens.Remove(token.name);
            tokenMenu.UnselectToken(token.name);
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