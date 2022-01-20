using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour 
{
	HexCell[] cells;
	public HexMesh terrain, water;
	static List<Vector3> colliderVertices = new List<Vector3>();
	static List<int> colliderTriangles= new List<int>();
	void Awake() 
	{
		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}
	
	void Start() 
	{
		Triangulate();
	}
	public void AddCell(int index, HexCell cell) 
	{
		cells[index] = cell;
		cell.transform.SetParent(transform, false);
	}
	public void Triangulate() 
    {
		terrain.Clear();
		water.Clear();
		for (int i = 0; i < cells.Length; i++) {
            
			Triangulate(cells[i]);
            TriangulateCollider(cells[i]);
		}
		terrain.Apply();
		water.Apply();
		water.PlanarUVProjection();
	}
    void Triangulate(HexCell cell) 
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) 
        {
			Triangulate(d, cell);
		}
	}

    void TriangulateCollider(HexCell cell)
    {
        Vector3 centerZero = new Vector3(0, HexMetrics.elevationStep, 0);
        Mesh newMesh = new Mesh();
        newMesh.name = "ColliderMesh";
        colliderVertices.Clear();
	    colliderTriangles.Clear();
        
		for (int i = 0; i < 6; i++) 
        {
            AddColliderTriangles(centerZero, centerZero + HexMetrics.corners[i], centerZero + HexMetrics.corners[i + 1]);
		}
        newMesh.vertices = colliderVertices.ToArray();
		newMesh.triangles = colliderTriangles.ToArray();
        newMesh.RecalculateNormals();
        cell.meshCollider.sharedMesh = newMesh;
    }

    void Triangulate(HexDirection direction, HexCell cell) 
    {
		Vector3 center = cell.Position;

		EdgeVertices e = new EdgeVertices(center + HexMetrics.GetFirstSolidCorner(direction),center + HexMetrics.GetSecondSolidCorner(direction));

		TriangulateEdgeFan(center, e, cell.color);

		if (direction <= HexDirection.SE) 
			TriangulateConnection(direction, cell, e);

		if (cell.IsUnderwater)
		{
			TriangulateWater(direction, cell, center);
		}

	} 
	void TriangulateWater(HexDirection direction, HexCell cell, Vector3 center) 
	{
		center.y = HexMetrics.waterSurfaceLevel;
		Vector3 c1 = center + HexMetrics.GetFirstSolidCorner(direction);
		Vector3 c2 = center + HexMetrics.GetSecondSolidCorner(direction);

		water.AddTriangle(center, c1, c2);

		if (direction <= HexDirection.SE) 
		{
			HexCell neighbor = cell.GetNeighbor(direction);
			if (neighbor == null || !neighbor.IsUnderwater)
				return;

			Vector3 bridge = HexMetrics.GetBridge(direction);
			Vector3 e1 = c1 + bridge;
			Vector3 e2 = c2 + bridge;

			water.AddQuad(c1, c2, e1, e2);

			if (direction <= HexDirection.E) 
			{
				HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
				if (nextNeighbor == null || !nextNeighbor.IsUnderwater)
					return;
				water.AddTriangle(c2, e2, c2 + HexMetrics.GetBridge(direction.Next()));
			}
		}
	}
	void TriangulateConnection(HexDirection direction, HexCell cell, EdgeVertices e1) 
	{
		HexCell neighbor = cell.GetNeighbor(direction);
		if (neighbor == null)
			return;

		Vector3 bridge = HexMetrics.GetBridge(direction);
		bridge.y = neighbor.Position.y - cell.Position.y;
		EdgeVertices e2 = new EdgeVertices(e1.v1 + bridge,e1.v4 + bridge);
		
		if (cell.GetEdgeType(direction) == HexEdgeType.Slope) 
			TriangulateEdgeTerraces(e1, cell, e2, neighbor);
		else
			TriangulateEdgeStrip(e1, cell.color, e2, neighbor.color);
		
		HexCell nextNeighbor = cell.GetNeighbor(direction.Next());
		if (direction <= HexDirection.E && nextNeighbor != null) 
		{
			Vector3 v5 = e1.v4 + HexMetrics.GetBridge(direction.Next());
			v5.y = nextNeighbor.Position.y;

			if (cell.Elevation <= neighbor.Elevation) {
				if (cell.Elevation <= nextNeighbor.Elevation) 
					TriangulateCorner(e1.v4, cell, e2.v4, neighbor, v5, nextNeighbor);
				else
					TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
			}
			else if (neighbor.Elevation <= nextNeighbor.Elevation)
				TriangulateCorner(e2.v4, neighbor, v5, nextNeighbor, e1.v4, cell);
			else
				TriangulateCorner(v5, nextNeighbor, e1.v4, cell, e2.v4, neighbor);
		}
	}
	void TriangulateEdgeFan (Vector3 center, EdgeVertices edge, Color color) 
	{
		terrain.AddTriangle(center, edge.v1, edge.v2);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v2, edge.v3);
		terrain.AddTriangleColor(color);
		terrain.AddTriangle(center, edge.v3, edge.v4);
		terrain.AddTriangleColor(color);
	}
	void TriangulateEdgeStrip(EdgeVertices e1, Color c1, EdgeVertices e2, Color c2) 
	{
		terrain.AddQuad(e1.v1, e1.v2, e2.v1, e2.v2);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v2, e1.v3, e2.v2, e2.v3);
		terrain.AddQuadColor(c1, c2);
		terrain.AddQuad(e1.v3, e1.v4, e2.v3, e2.v4);
		terrain.AddQuadColor(c1, c2);
	}

	void TriangulateEdgeTerraces (EdgeVertices begin, HexCell beginCell,EdgeVertices end, HexCell endCell) 
	{
		EdgeVertices e2 = EdgeVertices.TerraceLerp(begin, end, 1);
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, 1);

		TriangulateEdgeStrip(begin, beginCell.color, e2, c2);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			EdgeVertices e1 = e2;
			Color c1 = c2;
			e2 = EdgeVertices.TerraceLerp(begin, end, i);
			c2 = HexMetrics.TerraceLerp(beginCell.color, endCell.color, i);
			TriangulateEdgeStrip(e1, c1, e2, c2);
		}

		TriangulateEdgeStrip(e2, c2, end, endCell.color);
	}
	void TriangulateCorner(Vector3 bottom, HexCell bottomCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell) 
	{
		HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
		HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

		if (leftEdgeType == HexEdgeType.Slope) 
		{
			if (rightEdgeType == HexEdgeType.Slope) 
				TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
			else if (rightEdgeType == HexEdgeType.Flat) 
				TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
			else 
				TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
		}
		else if (rightEdgeType == HexEdgeType.Slope) 
		{
			if (leftEdgeType == HexEdgeType.Flat)
				TriangulateCornerTerraces( right, rightCell, bottom, bottomCell, left, leftCell);
			else
				TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
		}
		else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) 
		{
			if (leftCell.Elevation < rightCell.Elevation) 
				TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
			else
				TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
		}
		else 
		{
			terrain.AddTriangle(bottom, left, right);
			terrain.AddTriangleColor(bottomCell.color, leftCell.color, rightCell.color);
		}
	}
	void TriangulateCornerTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell) 
	{
		Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
		Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
		Color c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);
		Color c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, 1);

		terrain.AddTriangle(begin, v3, v4);
		terrain.AddTriangleColor(beginCell.color, c3, c4);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) {
			Vector3 v1 = v3;
			Vector3 v2 = v4;
			Color c1 = c3;
			Color c2 = c4;
			v3 = HexMetrics.TerraceLerp(begin, left, i);
			v4 = HexMetrics.TerraceLerp(begin, right, i);
			c3 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			c4 = HexMetrics.TerraceLerp(beginCell.color, rightCell.color, i);
			terrain.AddQuad(v1, v2, v3, v4);
			terrain.AddQuadColor(c1, c2, c3, c4);
		}

		terrain.AddQuad(v3, v4, left, right);
		terrain.AddQuadColor(c3, c4, leftCell.color, rightCell.color);
	}
	void TriangulateCornerTerracesCliff(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell) 
	{
		float b = 1f / (rightCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;
		Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(right), b);
		Color boundaryColor = Color.Lerp(beginCell.color, rightCell.color, b);

		TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);
		
		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) 
			TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
		else 
		{
			terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
			terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}
	void TriangulateCornerCliffTerraces(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 right, HexCell rightCell) 
	{
		float b = 1f / (leftCell.Elevation - beginCell.Elevation);
		if (b < 0) b = -b;

		Vector3 boundary = Vector3.Lerp(HexMetrics.Perturb(begin), HexMetrics.Perturb(left), b);
		Color boundaryColor = Color.Lerp(beginCell.color, leftCell.color, b);

		TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

		if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope) 
			TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
		else 
		{
			terrain.AddTriangleUnperturbed(HexMetrics.Perturb(left), HexMetrics.Perturb(right), boundary);
			terrain.AddTriangleColor(leftCell.color, rightCell.color, boundaryColor);
		}
	}
	void TriangulateBoundaryTriangle(Vector3 begin, HexCell beginCell, Vector3 left, HexCell leftCell, Vector3 boundary, Color boundaryColor) 
	{
		Vector3 v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, 1));
		Color c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, 1);

		terrain.AddTriangleUnperturbed(HexMetrics.Perturb(begin), v2, boundary);
		terrain.AddTriangleColor(beginCell.color, c2, boundaryColor);

		for (int i = 2; i < HexMetrics.terraceSteps; i++) 
		{
			Vector3 v1 = v2;
			Color c1 = c2;
			v2 = HexMetrics.Perturb(HexMetrics.TerraceLerp(begin, left, i));
			c2 = HexMetrics.TerraceLerp(beginCell.color, leftCell.color, i);
			terrain.AddTriangleUnperturbed(v1, v2, boundary);
			terrain.AddTriangleColor(c1, c2, boundaryColor);
		}

		terrain.AddTriangleUnperturbed(v2, HexMetrics.Perturb(left), boundary);
		terrain.AddTriangleColor(c2, leftCell.color, boundaryColor);
	}
	public void AddColliderTriangles(Vector3 v1, Vector3 v2, Vector3 v3) 
    {
		int vertexIndex = colliderVertices.Count;
		colliderVertices.Add(v1);
		colliderVertices.Add(v2);
		colliderVertices.Add(v3);
		colliderTriangles.Add(vertexIndex);
		colliderTriangles.Add(vertexIndex + 1);
		colliderTriangles.Add(vertexIndex + 2);
	}
}