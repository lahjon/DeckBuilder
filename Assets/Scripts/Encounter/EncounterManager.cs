using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EncounterManager : Manager
{
    public List<GameObject> actRoads; 
    public GameObject startPos;
    public GameObject UIPrefab;
    public Canvas canvas;
    public List<Encounter> overworldEncounters;
    public GameObject townEncounter;
    public Encounter currentEncounter;
    public Encounter currentEncounterHex;
    public int encounterTier;

    public GameObject ActTemplate;
    public GameObject RoadTemplate;
    public GameObject overWorldEncounterTemplate;
    
    public int maxWidth = 4;
    public int minWidth = 3;
    public int length = 7;
    public float placementNoise = 3.0f;
    public double branshProb = 0.25;
    public float tileSizeInverse;

    public GameObject templateHexEncounter; 

    [HideInInspector] public GameObject encounterParent; 

    protected override void Awake()
    {
        base.Awake();
        world.encounterManager = this;
    }
    protected override void Start()
    {
        base.Start();
        tileSizeInverse = 1 / world.gridManager.hexScale;
        //encounterParent = canvas.transform.GetChild(0).GetChild(0).gameObject;
        //roadParent = canvas.transform.GetChild(0).GetChild(1).gameObject;
    }
   
    private Vector3 getPositionNoise(float amplitude)
    {
        return new Vector3(Random.Range(0, amplitude), Random.Range(0, amplitude), 0);
    }


    public EncounterRoad AddRoad(Encounter fromEnc, Encounter toEnc, bool animate = false)
    {
        GameObject roadObj = Instantiate(RoadTemplate, fromEnc.tile.roadParent);
        EncounterRoad road = roadObj.GetComponent<EncounterRoad>();

        road.DrawRoad(fromEnc, toEnc, animate);
        fromEnc.roads.Add(road);
        toEnc.roads.Add(road);
        return road;
    }

    IEnumerator AnimateRoad(GameObject parent)
    {
        GridState state = world.gridManager.gridState;
        world.gridManager.animator.SetBool("IsAnimating", true);
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(.15f);
        }
        world.gridManager.animator.SetBool("IsAnimating", false);
    }

    public void GenerateInitialHexEncounters(HexTile tile)
    {
        List<Vector3Int> chosenEncountersSlots = new List<Vector3Int>();

        tile.availableDirections.ForEach(x => chosenEncountersSlots.Add(HexTile.DirectionToDoorEncounter(x)));

        chosenEncountersSlots.Add(Vector3Int.zero);

        for (int i = 0; i < chosenEncountersSlots.Count; i++)
        {
            GameObject obj = Instantiate(templateHexEncounter, tile.encounterParent);
            Encounter enc = obj.GetComponent<Encounter>();
            enc.Init();
            enc.coordinates = chosenEncountersSlots[i];
            enc.name = chosenEncountersSlots[i].ToString();
            enc.encounterType = i < tile.availableDirections.Count ? OverworldEncounterType.Exit :OverworldEncounterType.Start;
            enc.transform.localPosition = HexTile.EncounterPosToLocalCoord(chosenEncountersSlots[i]) + getPositionNoise(HexTile.encounterNoiseAllowed);
            enc.tile = tile;
            enc.status = EncounterHexStatus.Visited;
            tile.AddEncounter(chosenEncountersSlots[i], enc, i < tile.availableDirections.Count);
        }

        Encounter middleEnc = tile.posToEncounter[Vector3Int.zero];
        foreach(Encounter enc in tile.encountersExits)
        {
            enc.neighboors.Add(middleEnc);
            middleEnc.neighboors.Add(enc);
            EncounterRoad road = AddRoad(enc, middleEnc);
            road.status = EncounterRoadStatus.Traversed;
        }
        tile.OffsetRotation(true);
    }

    public void GenerateHexEncounters(HexTile tile, List<Vector3Int> mandatoryInnerSlots =  null, int additional = -1)
    {
        List<Vector3Int> EncounterSlots = new List<Vector3Int>(HexTile.positionsInner);
        List<Vector3Int> chosenEncountersSlots = new List<Vector3Int>();
        List<EncounterEdge> edges;

        Debug.Log("Starting Hex generate for tile " + tile.name + ", nr directions " + tile.availableDirections.Count);
        for (int i = 0; i < 6; i++)
        {
            if (!tile.availableDirections.Contains(i))
            {
                Vector3Int v = HexTile.DirectionToDoorEncounter(i);
                EncounterSlots.Add(v);
            }
        }


        tile.availableDirections.ForEach(x => chosenEncountersSlots.Add(HexTile.DirectionToDoorEncounter(x)));
        mandatoryInnerSlots?.ForEach(x => { chosenEncountersSlots.Add(x); EncounterSlots.Remove(x);});

        int nrAdditional = Random.Range(tile.availableDirections.Count, tile.availableDirections.Count*2);
        if(additional >= 0)
            nrAdditional = additional;
        //int nrAdditional = 100;

        for(int i = 0; i < nrAdditional && EncounterSlots.Count != 0; i++)
        {
            int index = Random.Range(0, EncounterSlots.Count);
            chosenEncountersSlots.Add(EncounterSlots[index]);
            EncounterSlots.RemoveAt(index);
        }

        for (int i = 0; i < chosenEncountersSlots.Count; i++)
        {
            GameObject obj = Instantiate(templateHexEncounter, tile.encounterParent) as GameObject;
            Encounter enc = obj.GetComponent<Encounter>();
            enc.Init();
            enc.coordinates = chosenEncountersSlots[i];
            enc.name = chosenEncountersSlots[i].ToString();
            enc.encounterType = i < tile.availableDirections.Count ? OverworldEncounterType.Exit : OverworldEncounterType.CombatNormal;
            enc.transform.localPosition = HexTile.EncounterPosToLocalCoord(chosenEncountersSlots[i])+ getPositionNoise(HexTile.encounterNoiseAllowed);
            enc.tile = tile;
            tile.AddEncounter(chosenEncountersSlots[i], enc, i < tile.availableDirections.Count);

        }


        HashSet<Vector3Int> occupiedSpaces = new HashSet<Vector3Int>(tile.posToEncounter.Keys);
        // Create initital non-crossing Paths. Taking copy since arguments is destructive
        edges = AssignNonCrossingEdges(tile.posToEncounter, tile.encountersExits);

        // Time to check if all nodes are connected to the same graph
        List<List<Encounter>> graphs = FindBiGraphs(new List<Encounter>(tile.encounters));
        if (graphs.Count > 1) ConnectBiGraphsNoCrosses(graphs, edges, occupiedSpaces);

        //Finally, time to see if it is possible to back into a corner
        
        List<Encounter> foundSubGraph = new List<Encounter>(); 
        foreach(Encounter n in tile.encounters)
        {
            n.status = EncounterHexStatus.Visited;
            List<Encounter> neighs = new List<Encounter>(n.neighboors);
            foreach(Encounter neigh in neighs)
            {
                if (!CanReachExitNode(neigh, tile.encountersExits.ToList(), foundSubGraph))
                {
                    Debug.Log("Subgraph size, first element:" + foundSubGraph.Count + "," + foundSubGraph[0].coordinates);
                    Connect2Graphs(foundSubGraph, tile.encounters.Except(foundSubGraph).ToList(), edges, occupiedSpaces);
                    foundSubGraph.Clear();
                }
            }

            n.status = EncounterHexStatus.Idle;
        }

        foreach (EncounterEdge e in edges)
        {
            AddRoad(e.n1, e.n2);
        }

        HexOptimizer optimizer = new HexOptimizer();
        optimizer.SetEncounters(tile.encounters.Except(tile.encountersExits).ToList());
        optimizer.Run();
        tile.OffsetRotation(true);
    }

    public void GenerateBossHexEncounter(HexTile tile)
    {
        List<Vector3Int> chosenEncountersSlots = new List<Vector3Int>();

        tile.availableDirections.ForEach(x => chosenEncountersSlots.Add(HexTile.DirectionToDoorEncounter(x)));

        chosenEncountersSlots.Add(Vector3Int.zero);

        for (int i = 0; i < chosenEncountersSlots.Count; i++)
        {
            GameObject obj = Instantiate(templateHexEncounter, tile.encounterParent);
            Encounter enc = obj.GetComponent<Encounter>();
            enc.Init();
            enc.coordinates = chosenEncountersSlots[i];
            enc.name = chosenEncountersSlots[i].ToString();
            enc.encounterType = i < tile.availableDirections.Count ? OverworldEncounterType.Exit : OverworldEncounterType.CombatBoss;
            enc.transform.localPosition = HexTile.EncounterPosToLocalCoord(chosenEncountersSlots[i]) + getPositionNoise(HexTile.encounterNoiseAllowed);
            enc.tile = tile;
            tile.AddEncounter(chosenEncountersSlots[i], enc, i < tile.availableDirections.Count);
        }

        Encounter middleEnc = tile.posToEncounter[Vector3Int.zero];

        foreach (Encounter enc in tile.encountersExits)
        {
            enc.neighboors.Add(middleEnc);
            middleEnc.neighboors.Add(enc);
            EncounterRoad road = AddRoad(enc, middleEnc);
        }
        tile.OffsetRotation(true);
    }

    private List<EncounterEdge> AssignNonCrossingEdges(Dictionary<Vector3Int, Encounter> nodes, List<Encounter> doorNodes)
    {
        Debug.Log("assign non crossing edges");
        List<EncounterEdge> edges = new List<EncounterEdge>();
        int nrNodes = nodes.Count;
        List<Encounter> allNodes = new List<Encounter>(nodes.Values);
        List<Encounter> allNodesNeigh = new List<Encounter>(allNodes);

        HashSet<Vector3Int> occupiedCoords = new HashSet<Vector3Int>(nodes.Keys);

        Shuffle(allNodes);
        foreach(Encounter enc in allNodes)
        {
            Shuffle(allNodesNeigh);
            foreach(Encounter neigh in allNodesNeigh)
            {
                if (neigh == enc || (doorNodes.Contains(enc) && doorNodes.Contains(neigh))) continue;
                EncounterEdge potentialEdge = new EncounterEdge(enc, neigh);
                if (edges.Contains(potentialEdge)) continue;

                if (NodeExistsBetween(enc.coordinates, neigh.coordinates, occupiedCoords))
                    continue;

                bool crosses = false;
                foreach (EncounterEdge edge in edges)
                {
                    if (PathCrosses(edge.GetNodePos(), potentialEdge.GetNodePos()))
                    {
                        crosses = true;
                        break;
                    }
                }
                if (crosses) continue;
                edges.Add(potentialEdge);
                enc.neighboors.Add(neigh);
                neigh.neighboors.Add(enc);
                break;
            }
        }
        return edges;
    }

    public bool IsCompleteGraph(List<Encounter> nodes)
    {
        List<Encounter> frontier = new List<Encounter>(nodes[0].neighboors);
        List<Encounter> visited = new List<Encounter>();
        visited.Add(nodes[0]);
        nodes.RemoveAt(0);
        while(frontier.Count != 0)
        {
            List<Encounter> newFrontier = new List<Encounter>();
            frontier.ForEach(x =>
            {
                visited.Add(x);
                newFrontier.AddRange(x.neighboors.Except(visited).Except(newFrontier));
                nodes.Remove(x);
            });
            frontier = newFrontier;
        }

        return nodes.Count == 0;
    }

    public List<List<Encounter>> FindBiGraphs(List<Encounter> nodes)
    {
        int gCount = 0;
        List<List<Encounter>> graphs = new List<List<Encounter>>();
        while(nodes.Count > 0)
        {
            Encounter cEntry = nodes[0];
            nodes.RemoveAt(0);
            graphs.Add(new List<Encounter>());
            graphs[gCount].Add(cEntry);
            List<Encounter> frontier = new List<Encounter>(cEntry.neighboors);
            while(frontier.Count != 0)
            {
                graphs[gCount].AddRange(frontier);
                List<Encounter> newFrontier = new List<Encounter>();
                frontier.ForEach(x => {
                    newFrontier.AddRange(x.neighboors.Except(graphs[gCount]).Except(newFrontier));
                    nodes.Remove(x);
                });
                frontier = newFrontier;
            }
            gCount++;
        }
        return graphs;
    }

    public bool CanReachExitNode(Encounter node, List<Encounter> doorNodes, List<Encounter> foundSubGraph = null)
    {
        if (doorNodes.Contains(node)) return true;

        List<Encounter> frontier = new List<Encounter>();
        List<Encounter> visited = new List<Encounter>();
        frontier.Add(node);
        while(frontier.Count != 0)
        {
            visited.AddRange(frontier);
            List<Encounter> newFrontier = new List<Encounter>();
            foreach(Encounter f in frontier)
            {
                if (doorNodes.Contains(f)) return true;
                newFrontier.AddRange(f.neighboors.Except(visited).Except(newFrontier).
                    Where(n => n.status != EncounterHexStatus.Unreachable && n.status != EncounterHexStatus.Visited));
            }
            frontier = newFrontier;
        }

        if (foundSubGraph != null) foundSubGraph.AddRange(visited);
        Debug.Log("Cant find exit from node: " + node);
        return false;
    }

    private void ConnectBiGraphsNoCrosses(List<List<Encounter>> graphs, List<EncounterEdge> currentEdges,HashSet<Vector3Int> coordsEncounters)
    {
        int limit = 10;
        while (graphs.Count > 1 && --limit > 0)
        {
            int gCount = graphs.Count;
            for(int i = 1; i < gCount; i++)
            {
                List<Encounter> g1 = new List<Encounter>(graphs[0]);
                List<Encounter> g2 = new List<Encounter>(graphs[i]);
                if(Connect2Graphs(g1,g2, currentEdges, coordsEncounters))
                {
                    graphs[0].AddRange(graphs[i]);
                    graphs.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private bool Connect2Graphs(List<Encounter> g1, List<Encounter> g2, List<EncounterEdge> currentEdges, HashSet<Vector3Int> coordsEncounters)
    {
        Shuffle(g1);
        Shuffle(g2);
        foreach (Encounter g1n in g1)
        {
            foreach (Encounter g2n in g2) 
            {
                Debug.Log("checking possible conn between:" + g1n.name + "," + g2n.name);
                //if (g1n == exceptEnc || g2n == exceptEnc) continue;
                EncounterEdge potentialEdge = new EncounterEdge(g1n, g2n);
                if (currentEdges.Count(e => e.Equals(potentialEdge)) > 0) continue;
                Debug.Log("edge didnt already exist");
                if (NodeExistsBetween(g1n.coordinates, g2n.coordinates, coordsEncounters))
                    continue;
                Debug.Log("Node didnt exist between");

                bool crosses = false;
                foreach (EncounterEdge edge in currentEdges)
                {
                    if (PathCrosses(potentialEdge.GetNodePos(), edge.GetNodePos()))
                    {
                        crosses = true;
                        Debug.Log("crosses edge: " + edge.n1.coordinates + "," + edge.n2.coordinates);
                        break;
                    }
                }
                

                if (!crosses)
                {
                    g1n.neighboors.Add(g2n);
                    g2n.neighboors.Add(g1n);
                    currentEdges.Add(potentialEdge);
                    return true;
                }
            }
        }
        Debug.Log("Couldnt connect two node sets!");
        return false;
    }

    public bool PathCrosses((Vector2 p1,Vector2 p2) e1, (Vector2 p3,Vector2 p4) e2)
    {
        float den = ((e1.p1.x - e1.p2.x) * (e2.p3.y - e2.p4.y) - (e1.p1.y - e1.p2.y) * (e2.p3.x - e2.p4.x));
        float t = ((e1.p1.x - e2.p3.x) * (e2.p3.y - e2.p4.y) - (e1.p1.y - e2.p3.y) * (e2.p3.x - e2.p4.x)) / den;
        float u = ((e1.p2.x - e1.p1.x) * (e1.p1.y - e2.p3.y) - (e1.p2.y - e1.p1.y) * (e1.p1.x - e2.p3.x)) / den;
        return t > 0.01 && t < 0.99 && u > 0.01 && u < 0.99;
    }

    public bool NodeExistsBetween(Vector3Int posA, Vector3Int posB, HashSet<Vector3Int> occupied)
    {

        Vector3Int starting;
        Vector3Int end;
        if (posA.x == posB.x)
        {
            starting =  posA.y < posB.y ? posA : posB;
            end =       posA.y < posB.y ? posB : posA;
            for (int i = 1; i < end.y - starting.y; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x,    starting.y + i,     starting.z - i)))
                    return true;
            }
        }
        else if (posA.y == posB.y)
        {
            starting =  posA.x < posB.x ? posA : posB;
            end =       posA.x < posB.x ? posB : posA;
            for (int i = 1; i < end.x - starting.x; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x + i,    starting.y,         starting.z - i)))
                    return true;
            }
        }
        else if (posA.z == posB.z)
        {
            starting =  posA.x < posB.x ? posA : posB;
            end =       posA.x < posB.x ? posB : posA;
            for (int i = 1; i < end.x - starting.x; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x + i,    starting.y -i,      starting.z)))
                    return true;
            }
        }

        LinearLineAxisType crossLine = HexCoordsOnLine(posA, posB);
        Debug.Log(crossLine);
        if (crossLine == LinearLineAxisType.Z)
        {
            starting = posA.x < posB.x ? posA : posB;
            end = posA.x < posB.x ? posB : posA;
            for (int i = 1; i < end.x - starting.x; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x + 1 * i, starting.y + 1 * i, starting.z - 2 * i)))
                    return true;
            }
        }

        if (crossLine == LinearLineAxisType.X)
        {
            starting = posA.x < posB.x ? posA : posB;
            end = posA.x < posB.x ? posB : posA;
            for (int i = 1; i < (end.x - starting.x)/2; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x + 2 * i, starting.y - 1 * i, starting.z - 1 * i)))
                    return true;
            }
        }

        if (crossLine == LinearLineAxisType.Y)
        {
            starting = posA.y < posB.y ? posA : posB;
            end = posA.y < posB.y ? posB : posA;
            for (int i = 1; i < (end.y - starting.y)/2; i++)
            {
                if (occupied.Contains(new Vector3Int(starting.x - 1 * i, starting.y + 2 * i, starting.z - 1 * i)))
                    return true;
            }
        }

        return false;
    }

    public HashSet<Encounter> FindAllReachableNodes(Encounter enc)
    {
        HashSet<Encounter> hs = new HashSet<Encounter>();

        if(enc.encounterType == OverworldEncounterType.Exit)
        {
            hs.Add(enc);
            return hs;
        }

        List<Encounter> neighs = enc.neighboors.Where(e => e.status == EncounterHexStatus.Idle || e.status == EncounterHexStatus.Selectable).ToList();
        foreach (Encounter neigh in neighs)
        {
            EncounterHexStatus originalStatus = neigh.status;
            //avoiding animationtriggers
            neigh._status = EncounterHexStatus.Visited;
            hs.UnionWith(FindAllReachableNodes(neigh));
            neigh._status = originalStatus;
        }

        if (hs.Count == 0) return hs;
        hs.Add(enc);
        return hs;
    }

    public void Testie()
    {
        HashSet<Encounter> hs = new HashSet<Encounter>();
        List<Encounter> list = new List<Encounter>();
        hs.UnionWith(list);
        hs.UnionWith(list);
        hs.UnionWith(list);
    }

    LinearLineAxisType HexCoordsOnLine(Vector3Int a, Vector3Int b)
    {
        Vector3Int starting = a.x < b.x ? a : b;
        Vector3Int end      = a.x < b.x ? b : a;
        for (int i = 1; i <= end.x - starting.x; i++)
            if (end == new Vector3Int(starting.x + i, starting.y + i, starting.z - 2 * i)) return LinearLineAxisType.Z;

        starting    = a.x < b.x ? a : b;
        end         = a.x < b.x ? b : a;
        for (int i = 1; i <= end.x - starting.x; i++)
            if (end == new Vector3Int(starting.x + 2 * i, starting.y - 1 * i, starting.z - 1 * i)) return LinearLineAxisType.X;


        starting    = a.y < b.y ? a : b;
        end         = a.y < b.y ? b : a;
        for (int i = 1; i < end.y - starting.y; i++)
            if (end == new Vector3Int(starting.x - 1 * i, starting.y + 2 * i, starting.z - 1 * i)) return LinearLineAxisType.Y;

        return LinearLineAxisType.NONE;
    }

    enum LinearLineAxisType
    {
        Z,
        Y,
        X,
        NONE
    }

    private static void Shuffle<T>(List<T> list)
    {
        for(int i = 0; i < list.Count;i++)
        {
            T temp = list[i];
            int index = Random.Range(i, list.Count);
            list[i] = list[index];
            list[index] = temp;
        }
    }


}
