using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class ArtifactMenu : MonoBehaviour
{
    public List<GameObject> allUIArtifacts = new List<GameObject>();
    public GameObject artifactUIPrefab;
    public Transform artifactContent;
    public Canvas canvas;

    public void Init()
    {

    }

    public void ToggleDisplay()
    {
        if (canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(false);    
        }
        else
        {
            canvas.gameObject.SetActive(true); 
        }
    }

    public GameObject AddUIArtifact(ArtifactData artifactData)
    {
        GameObject newArtifact = Instantiate(artifactUIPrefab, artifactContent);   
        Artifact artifact = newArtifact.GetComponent<Artifact>();
        artifact.itemData = artifactData;
        newArtifact.name = artifactData.itemName;
        allUIArtifacts.Add(newArtifact);
        return newArtifact;
    }

    public void RemoveUIArtifact(GameObject anArtifact)
    {
        foreach (GameObject artifact in allUIArtifacts)
        {
            if (anArtifact.name == artifact.name)
            {
                allUIArtifacts.Remove(artifact);
                Destroy(artifact);
                break;
            }
        }
    }

}
