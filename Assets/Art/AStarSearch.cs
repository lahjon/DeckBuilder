using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PriorityQueue<T>
{
    List<Tuple<T, int>> elements = new List<Tuple<T, int>>();
    public int Count => elements.Count;
    public void Enqueue(T item, int priority)
    {
        elements.Add(Tuple.Create(item, priority));
    }
    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++) {
            if (elements[i].Item2 < elements[bestIndex].Item2) {
                bestIndex = i;
            }
        }
        T bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
public static class AStarSearch
{
    public static Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
    public static Dictionary<HexTile, int> costSoFar = new Dictionary<HexTile, int>();
    public static List<HexTile> path = new List<HexTile>();

    static public int Heuristic(HexTile a, HexTile b)
    {
        return Math.Abs(a.coord.x - b.coord.x) + Math.Abs(a.coord.y - b.coord.y) + Math.Abs(a.coord.x +a.coord.y -b.coord.x -b.coord.y);
    }
    public static List<HexTile> StartAStarSearch(HexTile start, HexTile goal)
    {
        cameFrom.Clear();
        costSoFar.Clear();
        path.Clear();
        PriorityQueue<HexTile> frontier = new PriorityQueue<HexTile>();
        frontier.Enqueue(start, 0);

        cameFrom[start] = start;
        costSoFar[start] = 0;

        int counter = 0;
        while (frontier.Count > 0)
        {
            counter++; if (counter > 500) break;  // DEBUG: remove when done testing
            HexTile current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (HexTile next in current.neighbours.Where(x => x.Traverseable || x == goal)) // kanske skipa goal check för opti
            {
                int newCost = costSoFar[current] + next.cost;
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    int priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }
        
        return ReconstructPath(start, goal, cameFrom);
    }
    static List<HexTile> ReconstructPath(HexTile start, HexTile goal, Dictionary<HexTile, HexTile> cameFrom)
    {
        List<HexTile> path = new List<HexTile>();
        HexTile current = goal; 
        int counter = 0;
        while (current.coord != start.coord)
        {
            counter++; if (counter > 500) break; // DEBUG: remove when done testing
            path.Add(current);
            if (cameFrom.ContainsKey(current))
                current = cameFrom[current];
            else
                return null;
        }
        path.Reverse();
        return path;
    }
}