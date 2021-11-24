using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class ArtifactMenu : MonoBehaviour
{
    public List<Artifact> allUIArtifacts = new List<Artifact>();
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

    public Artifact AddUIArtifact(ArtifactData artifactData)
    {
        Artifact artifact = Instantiate(artifactUIPrefab, artifactContent).GetComponent<Artifact>();
        artifact.BindData(artifactData);
        artifact.gameObject.name = artifactData.itemName;
        allUIArtifacts.Add(artifact);
        return artifact;
    }

    public void RemoveUIArtifact(Artifact anArtifact)
    {
        foreach (Artifact artifact in allUIArtifacts)
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
