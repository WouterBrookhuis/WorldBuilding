using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AllBrush : Brush
{
    public override void Apply(Vector2Int userNode, GridTerrainData terrainData, NodeAction action)
    {
        var userNodeValue = terrainData[userNode.x, userNode.y];

        for(int x = 0; x < terrainData.Width; x++)
        {
            for(int y = 0; y < terrainData.Height; y++)
            {
                action(userNode, userNodeValue, new Vector2Int(x, y), terrainData);
            }
        }
    }

    public override void Draw(Vector2Int userNode, GridTerrain terrain)
    {
        var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(userNode.x, userNode.y));
        Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
    }
}
