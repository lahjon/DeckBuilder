using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardScreen : MonoBehaviour
{
    public GameObject artifactPrefab;
    public Transform anchor;
    public Canvas canvas;
    public GameObject tempObj;
    public System.Action callback;

    public void GetArtifactReward(ArtifactData artifactData)
    {
        tempObj = Instantiate(artifactPrefab, anchor);   
        tempObj.GetComponent<Artifact>().itemData = artifactData;
        callback = () => WorldSystem.instance.artifactManager.AddArtifact(artifactData.itemName);
        WorldStateSystem.SetInRewardScreen();
    }

    public void OpenScreen()
    {
        canvas.gameObject.SetActive(true);
    }

    public void ClearScreen()
    {
        if (tempObj != null)
            Destroy(tempObj);
            
        if (callback != null)
        {
            callback.Invoke();
            callback = null;
        }
        canvas.gameObject.SetActive(false);
    }
}
