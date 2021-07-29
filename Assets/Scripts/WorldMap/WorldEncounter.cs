using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldEncounter : MonoBehaviour
{
    WorldEncounterType _worldEncounterType;
    public WorldEncounterData worldEncounterData;
    public WorldEncounterType worldEncounterType
    {
        get => _worldEncounterType;
        set
        {
            _worldEncounterType = value;
            
            switch (_worldEncounterType)
            {
                case WorldEncounterType.Main:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                    GetComponent<Image>().color = Color.white;
                    break;
                case WorldEncounterType.Repeatable:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    GetComponent<Image>().color = Color.blue;
                    break;
                case WorldEncounterType.Special:
                    GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                    GetComponent<Image>().color = Color.green;
                    break;
                default:
                    break;
            }
        }
    }

    public void BindData()
    {
        worldEncounterType = worldEncounterData.type;
    }

    void Start()
    {
    }
}
