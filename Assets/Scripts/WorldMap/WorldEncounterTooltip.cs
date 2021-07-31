using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldEncounterTooltip : MonoBehaviour
{
    public TMP_Text encounterName;
    public Reward encounterReward;
    public GameObject tooltipAnchor;
    public void EnableTooltip(WorldEncounter worldEncounter)
    {
        tooltipAnchor.SetActive(true);
        tooltipAnchor.transform.position = worldEncounter.transform.position;
    }

    public void DisableTooltip()
    {
        tooltipAnchor.SetActive(false);
    }
}
