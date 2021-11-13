using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileCreator : MonoBehaviour
{
    [HideInInspector][SerializeField] public List<TileCreatorEncounter> allEncounters = new List<TileCreatorEncounter>();
    [SerializeField] public List<TileCreatorEncounter> allRemovedEncounters = new List<TileCreatorEncounter>();
    [HideInInspector][Range(.1f,1f)]public float minDistBetweenEnc;
    [HideInInspector]public Vector3 encounterPosition = Vector3.zero;
    [HideInInspector]public string encounterType = "None";
    [HideInInspector]public string encounterDirection = "";
    [HideInInspector]public string encounterNeighbours = "";
    [HideInInspector]public bool active;
    [HideInInspector]public int optionMode;
    [HideInInspector]public int optionType;
    [HideInInspector]public int optionExitDirection;
    [HideInInspector]public TileCreatorEncounter currentEnc;
    public GameObject encounterPrefab;
    public Transform encounterParent;
    public HexTile hexTile;


    public void AddEncounter(Vector3 encPos)
    {
        if (!allEncounters.Any(x => x.position == encPos))
        {
            for (int i = 0; i < allEncounters.Count; i++)
                if (Vector3.Distance(allEncounters[i].position, encPos) < minDistBetweenEnc && encPos != allEncounters[i].position)
                    return;
            allEncounters.Add(new TileCreatorEncounter(encPos, GetEncounterType(), GetGridDirection(), allEncounters.Count()));  
        }
    }

    public int GetEncounterIndex()
    {
        return allEncounters.IndexOf(allEncounters.FirstOrDefault(x => x.position == encounterPosition));
    }

    OverworldEncounterType GetEncounterType()
    {
        switch (optionType)
        {
            case 0:
                return OverworldEncounterType.None;
            case 1:
                return OverworldEncounterType.Exit;
            case 2:
                return OverworldEncounterType.Start;
            case 3:
                return OverworldEncounterType.Story;
            default:
                return OverworldEncounterType.None;
        }
    }
    public GridDirection GetGridDirection()
    {
        switch (optionExitDirection)
        {
            case 0:
                return GridDirection.East;
            case 1:
                return GridDirection.NorthEast;
            case 2:
                return GridDirection.NorthWest;
            case 3:
                return GridDirection.West;
            case 4:
                return GridDirection.SouthWest;
            case 5:
                return GridDirection.SouthEast;
            default:
                return GridDirection.East;
        }
    }

    public void MovePoint(int i, Vector3 pos)
    {
        allEncounters[i].position = pos;
    }

    public void RemovePoint(Vector3 encPos)
    {
        if (PositionToObject(encPos) is TileCreatorEncounter obj && obj.position == encPos)
        {
            allEncounters.Remove(obj);
        }
    }

    TileCreatorEncounter PositionToObject(Vector3 encPos)
    {
        return allEncounters.FirstOrDefault(x => x.position == encPos);
    }

    public void MakeNeighbour(TileCreatorEncounter enc)
    {
        if (currentEnc != null)
        {
            if (!enc.neighbourIndex.Contains(currentEnc.idx))
                enc.neighbourIndex.Add(currentEnc.idx);
            if (!currentEnc.neighbourIndex.Contains(enc.idx))
                currentEnc.neighbourIndex.Add(enc.idx);
        }
    }

    public void RemoveNeighbour(TileCreatorEncounter enc)
    {
        if (currentEnc != null)
        {
            if (enc.neighbourIndex.Contains(currentEnc.idx))
                enc.neighbourIndex.Remove(currentEnc.idx);
            if (currentEnc.neighbourIndex.Contains(enc.idx))
                currentEnc.neighbourIndex.Remove(enc.idx);
        }
    }
    public void MakeEncounterType(Vector3 encPos)
    {
        if (PositionToObject(encPos) is TileCreatorEncounter obj)
        {
            obj.overworldEncounterType = GetEncounterType();
            if (obj.overworldEncounterType == OverworldEncounterType.Exit)
                obj.direction = GetGridDirection();
            
        }
    }

    public void ResetEncounters()
    {
        allEncounters.Clear();
    }
}
