using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildManager : Manager
{
    static bool _buildMode;
    public BuildingOverworld[] buildings;
    public Material defaultMaterial, ghostedMaterial;
    public Transform parent;
    public BuildingOverworld currentBuilding;
    public static bool Following;
    public static bool BuildMode
    {
        get => _buildMode;
        set
        {
            _buildMode = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        world.buildManager = this;
    }
    protected override void Start()
    {
        base.Start();
    } 
    void Update()
    {
        if (Following && Input.GetMouseButton(1))
        {
            CancelBuilding();
        }
    }

    public void ToggleGhost(BuildingType buildingType)
    {
        Following = true;
        currentBuilding = buildings[(int)buildingType];
        currentBuilding.gameObject.SetActive(true);
        currentBuilding.transform.position = Vector3.zero;
        currentBuilding.meshRenderer.sharedMaterials[0] = ghostedMaterial;
        currentBuilding.meshRenderer.sharedMaterials[1] = ghostedMaterial;
        currentBuilding.meshRenderer.sharedMaterials[2] = ghostedMaterial;
    }

    public void CancelBuilding()
    {
        currentBuilding.gameObject.SetActive(false);
        currentBuilding = null;
        Following = false;
    }

    public void FollowGhost(Vector3 pos)
    {
        currentBuilding.transform.position = new Vector3(pos.x, 6, pos.z);
    }
    public void HideGhost()
    {
        currentBuilding.transform.position = Vector3.zero;
    }

    public void PlaceBuilding(HexCell cell)
    {
        cell.Blocked = true;
        BuildingOverworld building = Instantiate<BuildingOverworld>(currentBuilding, cell.transform);
        building.transform.localPosition = Vector3.zero;
        cell.building = building;
        building.meshRenderer.sharedMaterials[0] = defaultMaterial;
        building.meshRenderer.sharedMaterials[1] = defaultMaterial;
        building.meshRenderer.sharedMaterials[2] = defaultMaterial;
        CancelBuilding();
    }


}