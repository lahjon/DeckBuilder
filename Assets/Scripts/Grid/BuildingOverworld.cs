using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingOverworld : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public BuildingType buildingType;

    public void ButtonStartPlacement()
    {
        WorldSystem.instance.buildManager.ToggleGhost(buildingType);
    }

}