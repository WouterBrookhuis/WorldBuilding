using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridTerrainData
{
    public int this[int x, int y]
    {
        get => Nodes[x + y * Width];
        set => Nodes[x + y * Width] = value;
    }
    [HideInInspector]
    public int[] Nodes;
    [Tooltip("The length of an edge between two horizontal nodes a.k.a. the grid size.")]
    public float EdgeLength;
    [Tooltip("The height between each node level, your height step.")]
    public float StepHeight;

    public int Width;
    public int Height;

    public GridTerrainData() : this(33, 33, 1, 1)
    {
    }

    public GridTerrainData(int width, int height, int edgeLength, int stepHeight)
    {
        Width = width;
        Height = height;
        Nodes = new int[width * height];
        EdgeLength = edgeLength;
        StepHeight = stepHeight;
    }

    /// <summary>
    /// Returns the node level multiplied by the step height.
    /// No bounds checking is performed.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public float GetNodeHeightUnchecked(int x, int y)
    {
        return this[x, y] * StepHeight;
    }

    /// <summary>
    /// Returns the node position. 
    /// x and y are multiplied by the edge length and the node level by the step height.
    /// No bounds checking is performed.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetNodePositionUnchecked(int x, int y)
    {
        return new Vector3(x * EdgeLength, this[x, y] * StepHeight, y * EdgeLength);
    }

    /// <summary>
    /// Returns a clamped node coordinate that is closest to the given position in the xz plane.
    /// </summary>
    /// <param name="localPosition"></param>
    /// <returns></returns>
    public Vector2Int GetClosestNode(Vector3 localPosition)
    {
        var nodePosition = new Vector2Int(
            Mathf.RoundToInt(localPosition.x / EdgeLength),
            Mathf.RoundToInt(localPosition.z / EdgeLength));
        nodePosition.x = Mathf.Clamp(0, nodePosition.x, Width - 1);
        nodePosition.y = Mathf.Clamp(0, nodePosition.y, Height - 1);
        return nodePosition;
    }

    public Vector3 SnapToNode(Vector3 localPosition)
    {
        var node = GetClosestNode(localPosition);
        return GetNodePositionUnchecked(node.x, node.y);
    }
}
