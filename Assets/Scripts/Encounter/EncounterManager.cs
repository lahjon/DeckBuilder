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
    public EncounterHex currentEncounterHex;
    public int encounterTier;

    public GameObject ActTemplate;
    public GameObject RoadTemplate;
    public GameObject overWorldEncounterTemplate;
    
    public int maxWidth = 4;
    public int minWidth = 3;
    public int length = 7;
    public float placementNoise = 3.0f;
    public double branshProb = 0.25;

    public GameObject templateHexEncounter; 

    Encounter[][] encounters;

    [HideInInspector]
    public GameObject encounterParent; 
    [HideInInspector]
    public GameObject roadParent; 
    public GameObject roadImage;

    public Material roadMaterial;

    public Vector3 GetStartPositionEncounter()
    {
        return overworldEncounters[0].transform.position;
    }

    protected override void Awake()
    {
        base.Awake();
        world.encounterManager = this;
    }
    protected override void Start()
    {
        base.Start();
        encounterParent = canvas.transform.GetChild(0).GetChild(0).gameObject;
        roadParent = canvas.transform.GetChild(0).GetChild(1).gameObject;
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

    public void OpenOverworldMap()
    {
        canvas.gameObject.SetActive(true);
    }

    public void CloseOverworldMap()
    {
        canvas.gameObject.SetActive(false);
    }
    public void GenerateMap(int newMinWidth = 0, int newMaxWidth = 0, int newLength = 0)
    {
        if(newMinWidth > 0 && newMaxWidth > 0 && newLength > 0)
        {
            minWidth = newMinWidth;
            maxWidth = newMaxWidth;
            length = newLength;
        }

        canvas.gameObject.SetActive(true);
        encounters = new Encounter[length][];

        //Setup first
        encounters[0] = new Encounter[1];

        //SetupLast
        encounters[length - 1] = new Encounter[1];


        for (int i = 1; i < length - 1; i++)
            encounters[i] = new Encounter[Random.Range(minWidth, maxWidth + 1)];

        float vSpacing = 20.0f;
        float hSpacing = 20.0f;

        //Randomize out encounters
        for (int i = 0; i < encounters.Length; i++)
        {
            for (int j = 0; j < encounters[i].Length; j++)
            {
                GameObject newEnc = Instantiate(UIPrefab, encounterParent.transform, false);
                newEnc.name = string.Format("encounter_{0}_{1}", i, j);

                Vector3 pos = startPos.transform.position;

                Vector3 noise = getPositionNoise(placementNoise);
                if(i == 0 && j == 0)
                    noise = Vector3.zero;

                newEnc.transform.position = new Vector3(i * hSpacing, j * vSpacing - (encounters[i].Length - 1) * hSpacing / 2.0f ,0.04f)  + noise + pos;
                encounters[i][j] = newEnc.GetComponent<Encounter>();
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

        StartAddRoads(encounters[0][0]);
        StartCoroutine(encounters[0][0].Entering(() => { }));
    }

    public void AssignNeighbours(int floor, int unassigned_lb, int unassigned_ub, int lb, int ub)
    {
        //Debug.Log("Enter assign neightbors with: " + floor + "," + unassigned_lb + "," + unassigned_ub + "," + lb + "," + ub);
        if (unassigned_lb == unassigned_ub)
        {
            for (int i = lb; i <= ub; i++)
                encounters[floor][unassigned_lb].neighbourEncounters.Add(encounters[floor + 1][i]);
            return;
        }

        int chosenNode = Random.Range(unassigned_lb, unassigned_ub + 1);
        double placement = 1.0*(chosenNode - unassigned_lb) / (unassigned_ub - unassigned_lb);
        //Debug.Log("chosenNode is: " + chosenNode + ", and relative placement is:" + placement);

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
                //Debug.Log("RandomLeft!");
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
                //Debug.Log("RandomRight!");
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
        return new Vector3(Random.Range(0, amplitude), Random.Range(0, amplitude), 0);
    }


    // Function to add and place roads between all encounters that are connected. Assumes all encounters can be reached from the first.

    private void StartAddRoads(Encounter root)
    {
        for (int i = 0; i < encounters.Length; i ++) 
        {
            for (int j = 0; j < encounters[i].Length; j++) 
            {
                //Debug.Log(encounters[i][j]);
                foreach(Encounter enc in encounters[i][j].neighbourEncounters)
                {
                    DrawRoad(encounters[i][j], enc);
                }
            }
        }
    }

    public void DrawRoad(Encounter fromEnc, Encounter toEnc)
    {
        Vector3 from = fromEnc.transform.position;
        Vector3 to = toEnc.transform.position;
        float dist = Vector3.Distance(from, to);
        float width = 0.1f;

        float gap = 0.05f;
        float break_gap = 0.05f;
        float dist_t = break_gap + width;
        Vector3 dir = Vector3.Normalize(to - from);

        GameObject newRoadParent = new GameObject();
        newRoadParent.name = string.Format("road_{0}_to_{1}", fromEnc.gameObject.name, toEnc.gameObject.name);
        newRoadParent.transform.SetParent(fromEnc.transform);
        List<Encounter> roads = new List<Encounter>();
        roads.Add(fromEnc);
        roads.Add(toEnc);
        toEnc.roads.Add(newRoadParent, roads);
        int count = 0;

        while (dist_t < dist - break_gap)
        {
            GameObject newRoad = Instantiate(roadImage, newRoadParent.transform);
            newRoad.transform.position = from + dir * dist_t;
            float angle = Vector3.Angle(to-from, newRoad.transform.up);
            newRoad.transform.rotation = Quaternion.LookRotation(from, to);
            dist_t += gap + width;
            count++;
        }
    }

    public void GenerateHexEncounters(HexTile tile, List<Vector3Int> mandatoryInnerSlots =  null)
    {
        List<Vector3Int> EncounterSlots = new List<Vector3Int>(HexTile.positionsInner);
        List<Vector3Int> chosenEncountersSlots = new List<Vector3Int>();
        List<EdgeEncounter> edges;

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
        //int nrAdditional = 100;

        for(int i = 0; i < nrAdditional && EncounterSlots.Count != 0; i++)
        {
            int index = Random.Range(0, EncounterSlots.Count);
            chosenEncountersSlots.Add(EncounterSlots[index]);
            EncounterSlots.RemoveAt(index);
        }

        for (int i = 0; i < chosenEncountersSlots.Count; i++)
        {
            GameObject EnemyObject = Instantiate(templateHexEncounter, tile.encounterParent) as GameObject;
            EncounterHex enc = EnemyObject.GetComponent<EncounterHex>();
            enc.coordinates = chosenEncountersSlots[i];
            enc.name = chosenEncountersSlots[i].ToString();
            enc.encounterType = i < tile.availableDirections.Count ? OverworldEncounterType.Exit : (OverworldEncounterType)Random.Range(2, 6);
            enc.transform.localPosition = HexTile.EncounterPosToLocalCoord(chosenEncountersSlots[i])+ getPositionNoise(HexTile.encounterNoiseAllowed);
            enc.tile = tile;
            tile.AddEncounter(chosenEncountersSlots[i], enc, i < tile.availableDirections.Count);

        }

        // Create initital non-crossing Paths. Taking copy since arguments is destructive
        edges = AssignNonCrossingEdges(tile.posToEncounter, tile.encountersExits);

        // Time to check if all nodes are connected to the same graph
        List<List<EncounterHex>> graphs = FindBiGraphs(new List<EncounterHex>(tile.encounters));
        if (graphs.Count > 1) ConnectBiGraphsNoCrosses(graphs, edges, new HashSet<Vector3Int>(tile.posToEncounter.Keys));

        //Finally, time to see if it is possible to back into a corner
        
        List<EdgeEncounter> initialEdges = new List<EdgeEncounter>(edges);
        List<EncounterHex> subGraphLoop = new List<EncounterHex>(); 
        foreach(EncounterHex n in tile.encounters)
        {
            n.status = EncounterHexStatus.Visited;

            foreach(EncounterHex neigh in n.hexNeighboors)
            {
                if (!CanReachExitNode(neigh, tile.encountersExits, subGraphLoop))
                {
                    Connect2Graphs(subGraphLoop, tile.encounters.Except(subGraphLoop).ToList(), edges, new HashSet<Vector3Int>(tile.posToEncounter.Keys), n);
                    subGraphLoop.Clear();
                }
            }

            n.status = EncounterHexStatus.Idle;
        }


        foreach (EdgeEncounter e in edges)
        {
            DrawRoad(e.n1, e.n2);
            //Debug.DrawLine(e.n1.transform.position, e.n2.transform.position, Color.green, 100000, false);
        }
    }

    private List<EdgeEncounter> AssignNonCrossingEdges(Dictionary<Vector3Int, EncounterHex> nodes, List<EncounterHex> doorNodes)
    {
        Debug.Log("assign non crossing edges");
        List<EdgeEncounter> edges = new List<EdgeEncounter>();
        int nrNodes = nodes.Count;
        List<EncounterHex> allNodes = new List<EncounterHex>(nodes.Values);
        List<EncounterHex> allNodesNeigh = new List<EncounterHex>(allNodes);

        HashSet<Vector3Int> occupiedCoords = new HashSet<Vector3Int>(nodes.Keys);

        Shuffle(allNodes);
        foreach(EncounterHex enc in allNodes)
        {
            Shuffle(allNodesNeigh);
            foreach(EncounterHex neigh in allNodesNeigh)
            {
                if (neigh == enc || (doorNodes.Contains(enc) && doorNodes.Contains(neigh))) continue;
                EdgeEncounter potentialEdge = new EdgeEncounter(enc, neigh);
                if (edges.Contains(potentialEdge)) continue;

                if (NodeExistsBetween(enc.coordinates, neigh.coordinates, occupiedCoords))
                    continue;

                bool crosses = false;
                foreach (EdgeEncounter edge in edges)
                {
                    if (PathCrosses(edge.GetNodePos(), potentialEdge.GetNodePos()))
                    {
                        crosses = true;
                        break;
                    }
                }
                if (crosses) continue;
                edges.Add(potentialEdge);
                enc.hexNeighboors.Add(neigh);
                neigh.hexNeighboors.Add(enc);
                break;
            }
        }
        return edges;
    }

    public bool IsCompleteGraph(List<EncounterHex> nodes)
    {
        List<EncounterHex> frontier = new List<EncounterHex>(nodes[0].hexNeighboors);
        List<EncounterHex> visited = new List<EncounterHex>();
        visited.Add(nodes[0]);
        nodes.RemoveAt(0);
        while(frontier.Count != 0)
        {
            List<EncounterHex> newFrontier = new List<EncounterHex>();
            frontier.ForEach(x =>
            {
                visited.Add(x);
                newFrontier.AddRange(x.hexNeighboors.Except(visited).Except(newFrontier));
                nodes.Remove(x);
            });
            frontier = newFrontier;
        }

        return nodes.Count == 0;
    }

    public List<List<EncounterHex>> FindBiGraphs(List<EncounterHex> nodes)
    {
        int gCount = 0;
        List<List<EncounterHex>> graphs = new List<List<EncounterHex>>();
        while(nodes.Count > 0)
        {
            EncounterHex cEntry = nodes[0];
            nodes.RemoveAt(0);
            graphs.Add(new List<EncounterHex>());
            graphs[gCount].Add(cEntry);
            List<EncounterHex> frontier = new List<EncounterHex>(cEntry.hexNeighboors);
            while(frontier.Count != 0)
            {
                graphs[gCount].AddRange(frontier);
                List<EncounterHex> newFrontier = new List<EncounterHex>();
                frontier.ForEach(x => {
                    newFrontier.AddRange(x.hexNeighboors.Except(graphs[gCount]).Except(newFrontier));
                    nodes.Remove(x);
                });
                frontier = newFrontier;
            }
            gCount++;
        }
        return graphs;
    }

    public bool CanReachExitNode(EncounterHex node, List<EncounterHex> doorNodes, List<EncounterHex> foundSubGraph = null)
    {
        if (doorNodes.Contains(node)) return true;

        List<EncounterHex> frontier = new List<EncounterHex>();
        List<EncounterHex> visited = new List<EncounterHex>();
        frontier.Add(node);
        visited.Add(node);
        while(frontier.Count != 0)
        {
            List<EncounterHex> newFrontier = new List<EncounterHex>();
            visited.AddRange(frontier);
            foreach(EncounterHex f in frontier)
            {
                if (f.status == EncounterHexStatus.Unreachable || f.status == EncounterHexStatus.Visited) continue;
                if (doorNodes.Contains(f)) return true;
                newFrontier.AddRange(f.hexNeighboors.Except(visited).Except(newFrontier));
            }
            frontier = newFrontier;
        }

        if (foundSubGraph != null) foundSubGraph.AddRange(visited);
        Debug.Log("Cant find exit from node: " + node);
        return false;
    }

    private void ConnectBiGraphsNoCrosses(List<List<EncounterHex>> graphs, List<EdgeEncounter> currentEdges,HashSet<Vector3Int> coordsEncounters, EncounterHex exceptEnc = null)
    {
        while (graphs.Count > 1)
        {
            int gCount = graphs.Count;
            for(int i = 1; i < gCount; i++)
            {
                List<EncounterHex> g1 = new List<EncounterHex>(graphs[0]);
                List<EncounterHex> g2 = new List<EncounterHex>(graphs[i]);
                if(Connect2Graphs(g1,g2, currentEdges, coordsEncounters, exceptEnc))
                {
                    graphs[0].AddRange(graphs[i]);
                    graphs.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private bool Connect2Graphs(List<EncounterHex> g1, List<EncounterHex> g2, List<EdgeEncounter> currentEdges, HashSet<Vector3Int> coordsEncounters, EncounterHex exceptEnc = null)
    {
        Shuffle(g1);
        Shuffle(g2);

        foreach (EncounterHex g1n in g1)
        {
            foreach (EncounterHex g2n in g2) 
            {
                if (g1n == exceptEnc || g2n == exceptEnc) continue;
                EdgeEncounter potentialEdge = new EdgeEncounter(g1n, g2n);
            
                if (NodeExistsBetween(g1n.coordinates, g2n.coordinates, coordsEncounters))
                    continue;


                bool crosses = false;
                foreach (EdgeEncounter edge in currentEdges)
                {
                    if (PathCrosses(potentialEdge.GetNodePos(), edge.GetNodePos()))
                    {
                        crosses = true;
                        break;
                    }
                }

                if (!crosses)
                {
                    g1n.hexNeighboors.Add(g2n);
                    g2n.hexNeighboors.Add(g1n);
                    currentEdges.Add(potentialEdge);
                    return true;
                }
            }
        }
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
                if (occupied.Contains(new Vector3Int(starting.x + i,    starting.y -1,      starting.z)))
                    return true;
            }
        }

        starting = posA.x < posB.x ? posA : posB;
        end = posA.x < posB.x ? posB : posA;
        for (int i = 1; i < end.x - starting.x; i++)
        {
            if (occupied.Contains(new Vector3Int(starting.x + i, starting.y - i, starting.z - 2*i)))
                return true;
        }

        return false;
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
