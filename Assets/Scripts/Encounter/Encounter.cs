using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Animations;



public class Encounter : MonoBehaviour
{
    public HexTileOverworld tile;
    public List<Encounter> neighboors;
    public List<EncounterRoad> roads = new List<EncounterRoad>();
    public ScenarioEncounterType _encounterType;
    public int actionPointCost;

    public Vector3Int coordinates;
    public Vector3 startingScale;

    public static Color highlightColor = new Color(.5f, .5f, .5f, 1f);
    SpriteRenderer spriteRenderer;

    private Tween tweenAction1;
    private Tween tweenAction2;

    public string storyID;
    public EncounterData encData;
    public bool oneTime;

    public EncounterHexStatus _status = EncounterHexStatus.Idle;
    bool _highlighted;
    public ScenarioEncounterType encounterType
    {
        get
        {
            return _encounterType;
        }
        set
        {
            _encounterType = value;
            EncounterSetup setup = WorldSystem.instance.encounterManager.encounterSetups.FirstOrDefault(x => x.encounterType == _encounterType);
            oneTime = setup.oneTime;
            actionPointCost = setup.cost;
            if (setup.encounterMesh != null)
                Instantiate(setup.encounterMesh, transform);
        }
    }

    public void SetEncounterType(ScenarioEncounterType type)
    {
        _encounterType = type;
        spriteRenderer = GetComponent<SpriteRenderer>();
        List<Sprite> allSprites = GameObject.Find("DatabaseSystem").GetComponent<DatabaseSystem>().allOverworldIcons;
        Debug.Log(allSprites);
        Sprite sprite = allSprites.Where(x => x.name == string.Format("Overworld{0}", type.ToString())).FirstOrDefault();
        if (sprite == null)
            sprite = allSprites.FirstOrDefault(x => x.name == "OverworldPlain");
        spriteRenderer.sprite = sprite;
    }

    public EncounterHexStatus status
    {
        get
        {
            return _status;
        }
        set
        {
            _status = value;

            if (_status == EncounterHexStatus.Selectable)
            {
                spriteRenderer.color = Color.white;
            }
            else if (_status == EncounterHexStatus.Visited)
                spriteRenderer.color = new Color(.6f, .6f, .6f);
            else if (_status == EncounterHexStatus.Unreachable)
            {
                spriteRenderer.color = new Color(.8f, .8f, .8f);
                foreach (EncounterRoad road in roads)
                    if(road.status == EncounterRoadStatus.Idle)
                        road.status = EncounterRoadStatus.Unreachable;
            }
            else
                spriteRenderer.color = Color.white;
        }
    }

    public void Init()
    {
        startingScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartEncounter()
    {
        if (!string.IsNullOrEmpty(storyID))
        {
            if(encData is EncounterDataCombat encComb)
            {
                CombatSystem.instance.encounterData = encComb;
                WorldStateSystem.SetInCombat(true);
                return;
            }
            else if(encData is EncounterDataRandomEvent encEvent)
            {
                WorldSystem.instance.uiManager.encounterUI.encounterData = encEvent;
                WorldStateSystem.SetInChoice(true);
            }
        }

        switch (encounterType)
        {
            case ScenarioEncounterType.CombatNormal:
            case ScenarioEncounterType.CombatElite:
            case ScenarioEncounterType.CombatBoss:
                encData = CombatSystem.instance.encounterData = WorldSystem.instance.scenarioManager.GetRndEncounterCombat(encounterType);
                WorldStateSystem.SetInCombat(true);
                break;
            case ScenarioEncounterType.Shop:
                WorldStateSystem.SetInOverworldShop(true);
                break;
            case ScenarioEncounterType.Choice:
                encData = WorldSystem.instance.uiManager.encounterUI.encounterData = WorldSystem.instance.scenarioManager.GetRndEncounterChoice();
                WorldStateSystem.SetInChoice(true);
                break;
            case ScenarioEncounterType.Blacksmith:
                WorldStateSystem.SetInBlacksmith(true);
                break;
            case ScenarioEncounterType.Exit:
                WorldSystem.instance.scenarioMapManager.CompleteCurrentTile();
                break;
            case ScenarioEncounterType.Bonfire:
                WorldStateSystem.SetInBonfire(true);
                break;
            default:
                Debug.LogWarning(string.Format("Implement a case for: {0}", encounterType.ToString()));
                break;
        }

        WorldSystem.instance.scenarioMapManager.finishedEncounterToReport = this;
    }


}

[System.Serializable]
public struct EncounterSetup
{
    public ScenarioEncounterType encounterType;
    public GameObject encounterMesh;
    public bool oneTime;
    public int cost;

}
