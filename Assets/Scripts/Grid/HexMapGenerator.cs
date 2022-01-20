using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;

public class HexMapGenerator : MonoBehaviour
{
    public HexGrid grid;
    int cellCount;
    HexCellPriorityQueue searchFrontier;
	int searchFrontierPhase;

    void Start()
    {
        GenerateMap(3, 3);
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
        RaiseTerrain(25);
        for (int i = 0; i < cellCount; i++) {
			grid.GetCell(i).SearchPhase = 0;
		}
        for (int i = 0; i < grid.chunks.Count(); i++)
        {
            grid.chunks[i].Triangulate();
        }
	}

    void RaiseTerrain(int chunkSize) 
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
			current.Elevation = 2;
			size += 1;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor && neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = neighbor.coordinates.DistanceTo(center);
                    neighbor.SearchHeuristic = Random.value < 0.5f ? 1: 0;
					searchFrontier.Enqueue(neighbor);
				}
			}
		}
		searchFrontier.Clear();
	}

	HexCell GetRandomCell() 
    {
		return grid.GetCell(Random.Range(0, cellCount));
	}
}