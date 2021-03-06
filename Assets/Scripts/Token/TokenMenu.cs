using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TokenMenu : MonoBehaviour
{
    public Transform contentAll;
    public Transform contentActive;
    public List<GameObject> allTokens = new List<GameObject>();
    public List<GameObject> activeTokens = new List<GameObject>();
    WorldSystem world;
    TokenManager tokenManager;
    public List<GameObject> tokenPoints = new List<GameObject>();
    public Transform contentPoints;
    public TMP_Text pointAmountText;
    bool initialized = false;
    public TMP_Text tokenName;
    public TMP_Text tokenDescription;
    public GameObject textPanel;

    public void UpdatePoints()
    {
        int limit = tokenManager.availableTokenPoints;
        for (int i = 0; i < tokenPoints.Count; i++)
        {
            if (i < limit)
            {
                tokenPoints[i].SetActive(true);
            }
            else
            {
                tokenPoints[i].SetActive(false);
            }
        }
        pointAmountText.text = string.Format("{0}/{1}", tokenManager.availableTokenPoints, tokenManager.tokenPoints);
    }

    public void DisplayText(Token token)
    {
        textPanel.SetActive(true);
        tokenName.text = token.name;
        tokenDescription.text = token.description;
    }

    public void StopDisplayText()
    {
        textPanel.SetActive(false);
    }

    void OnEnable()
    {
        if (!initialized)
        {
            Init();
            LoadTokens();
            initialized = true;
        }
        if (world != null)
        {
            UpdateTokens(world.tokenManager.tokensRequiresUpdate);
        }
        StopDisplayText();
    }
    
    public void UpdateTokens(List<GameObject> tokens)
    {
        tokens?.ForEach(x => x.GetComponent<Token>().UnlockToken());
    }

    public void SelectToken(string tokenName)
    {
        foreach (GameObject token in activeTokens)
        {
            if (token.name == tokenName)
            {
                token.SetActive(true);
                break;
            }
        }
    }
    public void UnselectToken(string tokenName)
    {
        foreach (GameObject token in activeTokens)
        {
            if (token.name == tokenName)
            {
                token.SetActive(false);
                break;
            }
        }
    }

    public void Confirm()
    {
        gameObject.SetActive(false);
    }

    public void AddNewToken(GameObject token, bool newUnlock = false)
    {
        GameObject newToken = Instantiate(token);
        newToken.name = token.name;
        newToken.transform.SetParent(contentAll);
        allTokens.Add(newToken);
        Token aToken = newToken.GetComponent<Token>();
        aToken.Init();
        aToken.active = false;

        newToken = Instantiate(token);
        newToken.name = token.name;
        newToken.transform.SetParent(contentActive);
        activeTokens.Add(newToken);
        aToken = newToken.GetComponent<Token>();
        aToken.Init();
        aToken.active = true;
        if (newUnlock)
        {
            aToken.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        tokenManager = WorldSystem.instance.tokenManager;
        world = WorldSystem.instance;
        foreach (GameObject token in tokenManager.allTokens)
        {
            AddNewToken(token);
        }
        for (int i = 0; i < contentPoints.transform.childCount; i++)
        {
            GameObject token = contentPoints.transform.GetChild(i).gameObject;
            tokenPoints.Add(token);
            token.SetActive(false);
        }
    }


    public void LoadTokens()
    {
        foreach (GameObject token in allTokens)
        {
            Token aToken = token.GetComponent<Token>();
            if (tokenManager.allTokens[tokenManager.allTokensName.IndexOf(aToken.name)].GetComponent<Token>().unlocked)
            {
                aToken.UnlockToken();
                if (tokenManager.selectedTokensName.Contains(aToken.gameObject.name))
                {
                    aToken.SetSelected(true);
                }
            }
            else
            {
                aToken.LockToken();
            }
        }

        foreach (GameObject token in activeTokens)
        {
            Token aToken = token.GetComponent<Token>();
            if(tokenManager.selectedTokensName.Contains(aToken.gameObject.name))
            {
                aToken.gameObject.SetActive(true);
            }
            else
            {
                aToken.gameObject.SetActive(false);
            }
        }

        UpdatePoints();
    }
    
}