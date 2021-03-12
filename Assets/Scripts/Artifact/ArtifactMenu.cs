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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ToggleDisplay();
        }
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

    public void AddUIArtifact(GameObject anArtifact)
    {
        GameObject newArtifact = Instantiate(artifactUIPrefab);   
        newArtifact.transform.SetParent(artifactContent);
        newArtifact.name = anArtifact.name;
        newArtifact.GetComponent<Image>().sprite = anArtifact.GetComponent<Image>().sprite;
        allUIArtifacts.Add(newArtifact);
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
