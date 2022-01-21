using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

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
	[Range(0f, 0.4f)]
	public float sinkProbability = 0.2f;
	[Range(0, 2)]
	public int waterLevel = 1;
	[Range(1, 5)]
	public int elevationMaximum = 2;
	[Range(0, 20)]
	public int mapBorderX = 5;

	[Range(0, 20)]
	public int mapBorderZ = 5;
	int xMin, xMax, zMin, zMax;
    public HexGrid grid;
	[Range(20, 200)]
	public int mapSizeX = 20;
	[Range(20, 200)]
	public int mapSizeZ = 20;
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
        GenerateMap(mapSizeX, mapSizeZ);

		UnityEditor.AssetDatabase.CreateAsset(grid.chunks[0].terrain.hexMesh, "Assets/testMesh.obj" );
		UnityEditor.AssetDatabase.SaveAssets();
    }

	public void GenerateMap(int x, int z) 
    {
        cellCount = x * z;
		grid.CreateMap(x, z);
        grid.currentCell = grid.GetCell(0,0);

        if (searchFrontier == null) 
        {
			searchFrontier = new HexCellPriorityQueue();
		}
        for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).Elevation = -2;
		}

		xMin = mapBorderX;
		xMax = x - mapBorderX;
		zMin = mapBorderZ;
		zMax = z - mapBorderZ;

        CreateLand();
		// foreach (HexCell cell in GetAllBoundryCells())
		// {
		// 	cell.Elevation = -1;
		// }

        for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}

        for (int i = 0; i < grid.chunks.Count(); i++)
        {
            grid.chunks[i].Triangulate();
        }
	}
	void CreateLand () {
		int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
		int counter = 0;
		while (landBudget > 0) {
			int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
			counter++; if(counter > 100)
			{
				Debug.LogWarning("Create Error");
				break;
			}
			if (Random.value < sinkProbability) {
				landBudget = SinkTerrain(chunkSize, landBudget);
			}
			else {
				landBudget = RaiseTerrain(chunkSize, landBudget);
			}
		}
		CreateTerrainType(Random.Range(3, 15), 15);
		CreateTerrainType(Random.Range(3, 15), 15);
		CreateTerrainType(Random.Range(3, 15), 15);
		CreateTerrainType(Random.Range(3, 15), 15);
		CreateTerrainType(Random.Range(3, 15), 15);
		CreateTerrainType(Random.Range(3, 15), 15);
		// for (int i = 0; i < 5; i++)
		// {
		// }
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

	int SinkTerrain (int chunkSize, int budget) 
    {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();
		

		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int sink = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
			current.Elevation = originalElevation - sink;
			if (originalElevation < waterLevel &&current.Elevation >= waterLevel) {
				budget +=1;
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
	int CreateTerrainType(int chunkSize, int budget) 
    {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomLandCell();
		TileType newType = tileTypes[Random.Range(0, tileTypes.Length)];

		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int sink = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
            int originalElevation = current.Elevation;
			current.TileType = newType;
			if (current.Elevation > waterLevel ) budget--;
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

 	int RaiseTerrain (int chunkSize, int budget) {
		searchFrontierPhase += 1;
		HexCell firstCell = GetRandomCell();
		firstCell.SearchPhase = searchFrontierPhase;
		firstCell.Distance = 0;
		firstCell.SearchHeuristic = 0;
		searchFrontier.Enqueue(firstCell);
		HexCoordinates center = firstCell.coordinates;

		int rise = Random.value < highRiseProbability ? 2 : 1;
		int size = 0;
		while (size < chunkSize && searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue();
			int originalElevation = current.Elevation;
			int newElevation = originalElevation + rise;
			if (newElevation > elevationMaximum) {
				continue;
			}
			current.Elevation = newElevation;
			if (originalElevation < waterLevel && newElevation >= waterLevel && --budget == 0) {
				break;
			}
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
					neighbor.SearchHeuristic =
						Random.value < jitterProbability ? 1: 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
		return budget;
	}

	HexCell GetRandomCell() 
    {
		return grid.GetCell(Random.Range(xMin, xMax), Random.Range(zMin, zMax));
	}
	HexCell GetRandomLandCell() 
    {
		
		// for (int i = 0; i < grid.cells.Length; i++)
		List<HexCell> landCells = grid.cells.ToList().Where(x => x.Elevation > 0).ToList();
		return landCells.ElementAt(Random.Range(0, landCells.Count));
	}
	List<HexCell> GetAllBoundryCells() 
    {
		List<HexCell> boundryCells = new List<HexCell>();
		for (int i = 0; i < grid.cells.Length; i++)
		{

		}
		return boundryCells;
	}
}