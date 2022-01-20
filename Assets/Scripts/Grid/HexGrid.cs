using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    int cellCountX = 15;
	int cellCountZ = 15;
	public int chunkCountX = 4;
	public int chunkCountZ = 3;
	public HexCell cellPrefab;
	public HexGridChunk chunkPrefab;
    public TileSelector tileSelector;
    public static Color defaultColor = Color.white;
	public static Color touchedColor = Color.magenta;
    public List<Color> meshColors = new List<Color>();
	HexGridChunk[] chunks;
	public Texture2D noiseSource;
    HexCell[] cells;
    public static HexGrid instance;

	void Awake() 
    {
        if (instance == null) 
            instance = this;
        else 
            Destroy(gameObject);

		HexMetrics.noiseSource = noiseSource;

		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateChunks();
		CreateCells();
	}

	void OnEnable () 
	{
		HexMetrics.noiseSource = noiseSource;
	}
	void Start()
	{
		WorldSystem.instance.scenarioManager.scenarioCameraController.SetBounds(ClampPosition());
		
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
		cell.Elevation = Random.Range(0, 3);
		
		//position.y = HexMetrics.elevationStep * 2;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.name = string.Format("Tile: ({0}, {1})", x, z);
		//cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
        cell.color = meshColors[cell.Elevation];

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
}