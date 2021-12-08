using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class ArtifactMenu : MonoBehaviour
{
    public List<Artifact> allUIArtifacts = new List<Artifact>();
    public List<GameObject> artifactUIPrefabs;
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
        Debug.Log("Adding artifact!");
        Artifact artifact = Instantiate(artifactUIPrefabs[(int)artifactData.type], artifactContent).GetComponent<Artifact>();
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
