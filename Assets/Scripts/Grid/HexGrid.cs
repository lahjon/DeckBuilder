using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    public int cellCountX = 15;
	public int cellCountZ = 15;
	public int chunkCountX = 4;
	public int chunkCountZ = 3;
	public HexCell cellPrefab;
	HexCellPriorityQueue searchFrontier;
	int searchFrontierPhase;
	public HexGridChunk chunkPrefab;
    public TileSelector tileSelector;
    public static Color defaultColor = Color.white;
	public static Color touchedColor = Color.magenta;
    public List<Color> meshColors = new List<Color>();
	HexCell currentPathFrom, currentPathTo;
	bool currentPathExists;
	public HexMesh water;
	public HexGridChunk[] chunks;
	public Texture2D noiseSource;
    HexCell[] cells;
    public static HexGrid instance;
	public GameObject prefab;
	public HexCell currentCell;

	void Awake() 
    {
        if (instance == null) 
            instance = this;
        else 
            Destroy(gameObject);

		HexMetrics.noiseSource = noiseSource;
	}

	void OnEnable () 
	{
		HexMetrics.noiseSource = noiseSource;
	}

	public void CreateMap(int xCount, int zCount)
	{
		chunkCountX = xCount;
		chunkCountZ = zCount;
		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateChunks();
		CreateCells();

		WorldSystem.instance.scenarioManager.scenarioCameraController.SetBounds(ClampPosition());
		TriangulateWater();
	}
	public HexCell GetCell(int xOffset, int zOffset) 
	{
		return cells[xOffset + zOffset * cellCountX];
	}

	public HexGridChunk GetHexGridChunk(int chunkIndex)
	{
		return chunks[chunkIndex];
	}
	
	public HexCell GetCell(int cellIndex) 
	{
		return cells[cellIndex];
	}
    Vector3 ClampPosition() 
	{
		float xMax =(chunkCountX * HexMetrics.chunkSizeX - 0.5f) *(2f * HexMetrics.innerRadius);
		float zMax =(chunkCountZ * HexMetrics.chunkSizeZ - 1f) *(1.5f * HexMetrics.outerRadius);

		return new Vector3(xMax, 0, zMax);
	}
	void CreateChunks() 
	{
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}
	void CreateCells() 
	{
		cells = new HexCell[cellCountZ * cellCountX];
		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}	
	
	void CreateCell(int x, int z, int i) 
    {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
		
		//position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.elevationPerturbStrength;

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		
		//position.y = HexMetrics.elevationStep * 2;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = string.Format("Tile: ({0}, {1})", x, z);
		//cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;

        if (x > 0) 
		{
			cell.SetNeighbour(HexDirection.W, cells[i - 1]);
        }
        if (z > 0) 
		{
			if ((z & 1) == 0) 
			{
				cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) 
				{
					cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX - 1]);
				}
			}
			else 
			{
				cell.SetNeighbour(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) 
				{
					cell.SetNeighbour(HexDirection.SE, cells[i - cellCountX + 1]);
				}
			}
		}

		AddCellToChunk(x, z, cell);
	}
	void AddCellToChunk (int x, int z, HexCell cell) 
	{
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	void TriangulateWater()
    {
        Vector3 centerZero = new Vector3(0, HexMetrics.waterSurfaceLevel, 0);
		float zSize = HexGrid.instance.chunkCountZ * HexMetrics.chunkSizeZ * HexMetrics.innerRadius * 2;
		float xSize = HexGrid.instance.chunkCountX * HexMetrics.chunkSizeX * HexMetrics.outerRadius * 2;
		int density = 100;

		Vector3[] vertices = new Vector3[(density + 1) * (density + 1)];

        for (int i = 0, z = 0; z <= density; z++)
        {
            for (int x = 0; x <= density; x++)
            {
                vertices[i] = new Vector3(((float)x / density) * xSize, 0, ((float)z / density) * zSize);
                i++;
            }
        }

        int[] triangles = new int[density * density * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < density; z++)
        {
            for (int x = 0; x < density; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + density + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + density + 1;
                triangles[tris + 5] = vert + density + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

		water.hexMesh.vertices = vertices;
		water.hexMesh.triangles = triangles;
		water.hexMesh.name = "WaterMesh";
		water.hexMesh.RecalculateNormals();
		water.transform.localPosition = new Vector3(-30, 7, -30);
		water.PlanarUVProjection();
    }
	public void ShowPath () 
	{
		if (currentPathExists) {
			Debug.Log("SHOWING");
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.EnableHighlight();
				current = current.PathFrom;
				Debug.Log(current.coordinates);
			}
		}
		currentPathFrom.EnableHighlight();
		currentPathTo.EnableHighlight();
	}
	public void ClearPath () {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.DisableHighlight();
				current = current.PathFrom;
			}
			current.DisableHighlight();
			currentPathExists = false;
		}
		else if (currentPathFrom) {
			currentPathFrom.DisableHighlight();
			currentPathTo.DisableHighlight();
		}
		currentPathFrom = currentPathTo = null;
	}
	public bool FindPath(HexCell fromCell, HexCell toCell) 
	{
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		searchFrontierPhase += 2;
		for (int i = 0; i < cells.Length; i++) 
		{
			cells[i].Distance = int.MaxValue;
		}
		searchFrontier = new HexCellPriorityQueue();
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue(fromCell);
		while (searchFrontier.Count > 0) 
		{
			HexCell current = searchFrontier.Dequeue();
			current.SearchPhase += 1;

			if (current == toCell) 
			{
				currentPathExists = true;
				return true;
			}
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbour = current.GetNeighbor(d);
				if (neighbour == null ||neighbour.SearchPhase > searchFrontierPhase)
				{
					continue;
				}
				// if (neighbour.IsUnderwater) {
				// 	continue;
				// }
				if (current.GetEdgeType(neighbour) == HexEdgeType.Cliff) {
					continue;
				}
				int distance = current.Distance;

				if (neighbour.SearchPhase < searchFrontierPhase) {
					neighbour.SearchPhase = searchFrontierPhase;
					neighbour.Distance = distance;
					neighbour.PathFrom = current;
					neighbour.SearchHeuristic = neighbour.coordinates.DistanceTo(toCell.coordinates);
					searchFrontier.Enqueue(neighbour);
				}
				else if (distance < neighbour.Distance) {
					int oldPriority = neighbour.SearchPriority;
					neighbour.Distance = distance;
					neighbour.PathFrom = current;
					searchFrontier.Change(neighbour, oldPriority);
				}
			}
		}
		return false;
	}
}