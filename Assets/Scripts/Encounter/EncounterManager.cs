using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : MonoBehaviour
{
    public List<GameObject> actRoads; 
    public List<Encounter> overworldEncounters;
    public GameObject townEncounter;
    public Encounter currentEncounter;
    public int encounterTier;

    public GameObject ActTemplate;
    public GameObject RoadTemplate;
    public GameObject overWorldEncounterTemplate;
    
    public int maxWidth = 4;
    public int minWidth = 3;
    public int length = 7;
    public float placementNoise = 3.0f;
    public double branshProb = 0.25;


    EncounterOverworld[][] encounters;

    [HideInInspector]
    public GameObject EncounterParent; 

    public Vector3 GetStartPositionEncounter()
    {
        return overworldEncounters[0].transform.position;
    }


    public void Start()
    {
        EncounterParent = ActTemplate.transform.GetChild(0).GetChild(0).gameObject;
        GenerateMap();
        AddRoads(encounters[0][0]);
        encounters[0][0].SetIsVisited(false);
    }

    public void UpdateAllTownEncounters(int act)
    {
        encounterTier = act;
        Transform t = townEncounter.transform;
        for (int i = 0; i < t.childCount; i++)
        {   
            Encounter e = t.GetChild(i).gameObject.GetComponent<Encounter>();
            e.UpdateEncounter();
        }
    }


    public void GenerateMap()
    {
        encounters = new EncounterOverworld[length][];

        //Setup first
        encounters[0] = new EncounterOverworld[1];

        //SetupLast
        encounters[length - 1] = new EncounterOverworld[1];


        for (int i = 1; i < length - 1; i++)
            encounters[i] = new EncounterOverworld[Random.Range(minWidth, maxWidth + 1)];

        float vSpacing = 7.0f;

        //Randomize out encounters
        for (int i = 0; i < encounters.Length; i++)
        {
            for (int j = 0; j < encounters[i].Length; j++)
            {
                GameObject newEnc = Instantiate(overWorldEncounterTemplate, EncounterParent.transform, false);
                newEnc.transform.localPosition = new Vector3(i * 5, 0.04f, j * vSpacing - (encounters[i].Length - 1) * 5.0f / 2.0f) + getPositionNoise(placementNoise);
                encounters[i][j] = newEnc.GetComponent<EncounterOverworld>();
                overworldEncounters.Add(encounters[i][j]);
            }
        }
        
        for (int i = 0; i < encounters.Length; i++)
        {
            for (int j = 0; j < encounters[i].Length; j++)
            {
                encounters[i][j].encounterData = (i == encounters.Length - 1 ? DatabaseSystem.instance.GetRandomEncounterBoss() : DatabaseSystem.instance.GetRandomEncounter());
                encounters[i][j].UpdateEncounter();
            }
        }



        for (int i = 0; i < encounters.Length-1; i++)
        {
            AssignNeighbours(i, 0, encounters[i].Length - 1, 0, encounters[i+1].Length-1);
        }
    }

    public void AssignNeighbours(int floor, int unassigned_lb, int unassigned_ub, int lb, int ub)
    {
        Debug.Log("Enter assign neightbors with: " + floor + "," + unassigned_lb + "," + unassigned_ub + "," + lb + "," + ub);
        if (unassigned_lb == unassigned_ub)
        {
            for (int i = lb; i <= ub; i++)
                encounters[floor][unassigned_lb].neighbourEncounters.Add(encounters[floor + 1][i]);
            return;
        }

        int chosenNode = Random.Range(unassigned_lb, unassigned_ub + 1);
        double placement = 1.0*(chosenNode - unassigned_lb) / (unassigned_ub - unassigned_lb);
        Debug.Log("chosenNode is: " + chosenNode + ", and relative placement is:" + placement);

        int target = 0;
        int bonusLeft = 0;
        int bonusRight = 0;

        for(int i = lb; i <= ub; i++)
        {
            if(1.0 * (i - lb) / (ub - lb) >= placement)
            {
                target = i;
                break;
            }
        }

        encounters[floor][chosenNode].neighbourEncounters.Add(encounters[floor + 1][target]);

        //time to see if we get some bonus connections 
        for(int i = target-1; i >= lb; i--)
        {
            if (Random.Range(0, 1f) < branshProb)
            {
                encounters[floor][chosenNode].neighbourEncounters.Add(encounters[floor + 1][i]);
                Debug.Log("RandomLeft!");
                bonusLeft--;
            }
            else
                break;
        }

        for (int i = target + 1; i <= ub; i++)
        {
            if (Random.Range(0, 1f) < branshProb)
            {
                encounters[floor][chosenNode].neighbourEncounters.Add(encounters[floor + 1][i]);
                Debug.Log("RandomRight!");
                bonusRight++;
            }
            else
                break;
        }

        if(chosenNode != unassigned_lb) AssignNeighbours(floor, unassigned_lb, chosenNode - 1, lb, target + bonusLeft);
        if(chosenNode != unassigned_ub) AssignNeighbours(floor, chosenNode + 1, unassigned_ub, target + bonusRight, ub);
    }

    private Vector3 getPositionNoise(float amplitude)
    {
        return new Vector3(Random.Range(0, amplitude), 0, Random.Range(0, amplitude));
    }


    // Function to add and place roads between all encounters that are connected. Assumes all encounters can be reached from the first.
    public void AddRoads(Encounter root)
    {
        foreach(Encounter enc in root.neighbourEncounters)
        {
            AddRoad(root.transform.localPosition, enc.transform.localPosition);
            AddRoads(enc);
        }
    }

    public void AddRoad(Vector3 from, Vector3 to)
    {
        GameObject newRoad = Instantiate(RoadTemplate, actRoads[0].transform,false);
        float height = newRoad.transform.localPosition.y;

        Transform roadTrans = newRoad.transform;

        Vector3 newPos = from + ((to - from) / 2.0f);
        roadTrans.localPosition = from + ((to- from)/2.0f);
        roadTrans.localPosition = new Vector3(roadTrans.localPosition.x, height, roadTrans.localPosition.z );
        roadTrans.localScale = new Vector3(roadTrans.localScale.x, roadTrans.localScale.y, Vector3.Distance(from, to));

        float angle = Vector3.Angle(to-from  , roadTrans.forward);
        roadTrans.Rotate(new Vector3(0, angle, 0));
    }

}
