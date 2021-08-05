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
    WorldSystem world;
    TokenManager tokenManager;
    public List<GameObject> tokenPoints = new List<GameObject>();
    public Transform contentPoints;
    public TMP_Text pointAmountText;
    public TMP_Text tokenName;
    public TMP_Text tokenDescription;
    public GameObject textPanel;

    void OnEnable()
    {
        if (world != null)
        {
            UpdateTokens(world.tokenManager.tokensRequireUpdate);
        }
        StopDisplayText();
    }
    
    public void Init()
    {
        gameObject.SetActive(true);
        tokenManager = WorldSystem.instance.tokenManager;
        world = WorldSystem.instance;

        foreach (string tokenName in tokenManager.selectedTokens)
        {
            GameObject token = tokenManager.GetTokenByName(tokenName, tokenManager.allTokens);
            token.GetComponent<Token>().SetSelected(true);
        }

        for (int i = 0; i < contentPoints.transform.childCount; i++)
        {
            GameObject token = contentPoints.transform.GetChild(i).gameObject;
            tokenPoints.Add(token);
            token.SetActive(false);
        }
        UpdatePoints();
        gameObject.SetActive(false);
    }

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


    
    public void UpdateTokens(List<string> tokens)
    {
        foreach (GameObject token in tokenManager.allTokens)
        {
            if (tokens.Contains(token.name))
            {
                token.GetComponent<Token>().UnlockToken();
            }
            token.transform.localScale = Vector3.one;
        }
    }

    public void SelectToken(GameObject token)
    {
        if (token == null)
        {
            return;
        }

        GameObject activeToken = Instantiate(token);
        activeToken.name = token.name;
        activeToken.transform.SetParent(contentActive);
        activeToken.transform.localScale = Vector3.one;
        activeToken.transform.localPosition = Vector3.zero;
        activeToken.GetComponent<Token>().Init(true);

    }
    public void UnselectToken(string tokenName)
    {
        List<GameObject> tokens = new List<GameObject>();

        for (int i = 0; i < contentActive.transform.childCount; i++)
        {
            tokens.Add(contentActive.transform.GetChild(i).gameObject);
        }

        GameObject token = tokenManager.GetTokenByName(tokenName, tokens);

        if (token != null)
        {
            Destroy(token);
        }

    }

    public void Confirm()
    {
        gameObject.SetActive(false);
    }


    
}