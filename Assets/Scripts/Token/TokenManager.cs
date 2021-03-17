using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class TokenManager : Manager, ISaveableWorld, ISaveableTemp, ISaveableStart
{
    public List<GameObject> allTokens = new List<GameObject>();
    public List<GameObject> selectedTokens = new List<GameObject>();
    public List<string> allTokensName = new List<string>();
    public List<string> unlockedTokens = new List<string>();
    public List<string> selectedTokensName = new List<string>();
    public TokenMenu tokenMenu;
    public List<GameObject> tokensRequiresUpdate = new List<GameObject>();
    public int tokenPoints = 1;
    public int availableTokenPoints = 1;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (tokenMenu.gameObject.activeSelf)
            {
                tokenMenu.gameObject.SetActive(false);    
            }
            else
            {
                tokenMenu.gameObject.SetActive(true); 
            }
        }
    }

    public void UnlockNewToken(GameObject token)
    {
        unlockedTokens.Add(token.name);
        tokensRequiresUpdate.Add(token);
        allTokens[allTokensName.IndexOf(token.name)].GetComponent<Token>().UnlockToken();;
        world.SaveProgression();
    }

    public void AddTokenPoint()
    {
        tokenPoints++;
        availableTokenPoints++;
        tokenMenu.UpdatePoints();
        world.SaveProgression();
    }

    public void AddSelectedToken(string token, bool addNames = true)
    {
        GameObject aToken = allTokens[allTokensName.IndexOf(token)];
        selectedTokens.Add(aToken);
        aToken.GetComponent<Token>().AddActivity();
        
        if (addNames)
        {
            selectedTokensName.Add(aToken.name);
        }
        tokenMenu.SelectToken(token);
        
    }

    public void RemoveSelectedToken(string token)
    {
        Debug.Log("Remove");
        GameObject aToken = allTokens[allTokensName.IndexOf(token)];
        if (selectedTokens.Contains(aToken))
        {
            selectedTokens.Remove(aToken);
            aToken.GetComponent<Token>().RemoveActivity();
            selectedTokensName.Remove(aToken.name);
            tokenMenu.UnselectToken(token);
        }
    }

    public void Init()
    { 
        allTokensName = allTokens.ConvertAll(x => x.name);

        if (SceneManager.GetActiveScene().buildIndex == 0 && selectedTokensName.Count == 0 && availableTokenPoints == 0)
        {
            availableTokenPoints = tokenPoints;
        }

        GameObject token;
        selectedTokens.Clear();
        foreach (string tokenName in selectedTokensName)
        {
            token = allTokens[allTokensName.IndexOf(tokenName)];
            AddSelectedToken(token.name, false);
        }
        tokenMenu.Init();
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.selectedTokens = selectedTokensName;
        a_SaveData.availableTokenPoints = availableTokenPoints;
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        availableTokenPoints = a_SaveData.availableTokenPoints;
        selectedTokensName = a_SaveData.selectedTokens;
    }

    public void PopulateSaveDataStart(SaveDataStart a_SaveData)
    {
        a_SaveData.selectedTokens = selectedTokensName;
        a_SaveData.availableTokenPoints = availableTokenPoints;
    }

    public void LoadFromSaveDataStart(SaveDataStart a_SaveData)
    {
        availableTokenPoints = a_SaveData.availableTokenPoints;
        selectedTokensName = a_SaveData.selectedTokens;
    }
}