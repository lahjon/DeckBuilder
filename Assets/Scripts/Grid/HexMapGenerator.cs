using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexMapGenerator : MonoBehaviour
{
    [Range(20, 200)]
	public int chunkSizeMin = 30;

	[Range(20, 200)]
	public int chunkSizeMax = 100;
    [Range(5, 95)]
	public int landPercentage = 50;
    [Range(0f, 1f)]
	public float highRiseProbability = 0.25f;
    public HexGrid grid;
    int cellCount;
    HexCellPriorityQueue searchFrontier;
	int searchFrontierPhase;
	[Range(0f, 0.5f)] public float jitterProbability = 0.25f;
	TileType[] tileTypes = new TileType[] {
		TileType.Sand,
		TileType.Grass,
		TileType.Plains,
		TileType.Forest,
		TileType.Mountain,
		TileType.Snow
	};

    void Start()
    {
        GenerateMap(5,5);
    }

	public void GenerateMap(int x, int z) 
    {
        cellCount = x * HexMetrics.chunkSizeX * z * HexMetrics.chunkSizeZ;
		grid.CreateMap(x, z);
        grid.currentCell = grid.GetCell(0,0);

        if (searchFrontier == null) 
        {
			searchFrontier = new HexCellPriorityQueue();
		}
        for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).Elevation = 0;
		}

        CreateLand();
		//SetTerrainType(10);
		SinkWater(20);
		RaiseHills(5);
		SinkHoles(5);

        for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}

        for (int i = 0; i < grid.chunks.Count(); i++)
        {
            grid.chunks[i].Triangulate();
        }
	}
    void CreateLand() {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
        while (landBudget > 0) {
			landBudget = RaiseTerrain(
				Random.Range(chunkSizeMin, chunkSizeMax + 1), landBudget
			);
		}
	}

	void SetTerrainType(int percentage)
	{
		for (int i = 0; i < percentage; i++) {
			GetRandomCell().TileType = tileTypes[Random.Range(0, tileTypes.Length - 1) + 1];
		}
	}
	void SinkWater(float percentage)
	{
		List<HexCell> waterTiles = new List<HexCell>();
		for (int i = 0; i < grid.cells.Length; i++)
		{
			if (grid.cells[i].Elevation < 1)
			{
				waterTiles.Add(grid.cells[i]);
			}
		}
		int limit = (int)(waterTiles.Count * (percentage / 100));
		while (waterTiles.Count > limit)
		{
			waterTiles.RemoveAt(Random.Range(0, waterTiles.Count));
		}
		for (int i = 0; i < waterTiles.Count; i++) {
			waterTiles[i].Elevation = -1;
		}
	}

	void SinkHoles(float percentage)
	{
		List<HexCell> waterTiles = new List<HexCell>();
		for (int i = 0; i < grid.cells.Length; i++)
		{
			if (grid.cells[i].Elevation == 1)
			{
				waterTiles.Add(grid.cells[i]);
			}
		}
		int limit = (int)(waterTiles.Count * (percentage / 100));
		while (waterTiles.Count > limit)
		{
			waterTiles.RemoveAt(Random.Range(0, waterTiles.Count));
		}
		for (int i = 0; i < waterTiles.Count; i++) {
			waterTiles[i].Elevation = 0;
		}
	}

	void RaiseHills(float amount)
	{
		List<HexCell> hillTiles = new List<HexCell>();
		for (int i = 0; i < grid.cells.Length; i++)
		{
			if (grid.cells[i].Elevation > 0)
			{
				hillTiles.Add(grid.cells[i]);
			}
		}
		int limit = (int)(hillTiles.Count * (amount / 100));
		while (hillTiles.Count > limit)
		{
			hillTiles.RemoveAt(Random.Range(0, hillTiles.Count));
		}
		for (int i = 0; i < hillTiles.Count; i++) {
			hillTiles[i].Elevation = 2;
		}
	}

    int RaiseTerrain (int chunkSize, int budget) 
    {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();

		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
            current.Elevation = 1;
			if (current.Elevation == 1 && --budget == 0) {
				break;
			}
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) 
				{
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic = Random.value < jitterProbability ? 1: 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
        return budget;
	}

	HexCell GetRandomCell() 
    {
		return grid.GetCell(Random.Range(0, cellCount));
	}
}