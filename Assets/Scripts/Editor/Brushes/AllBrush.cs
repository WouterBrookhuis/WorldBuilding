using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AllBrush : Brush
{
    public override void Apply(Vector2Int userNode, SerializedProperty nodes, GridTerrain terrain, NodeAction action)
    {
        var userNodeValue = nodes.GetArrayElementAtIndex(userNode.x + userNode.y * terrain.TerrainData.Width).intValue;

        for(int x = 0; x < terrain.TerrainData.Width; x++)
        {
            for(int y = 0; y < terrain.TerrainData.Height; y++)
            {
                action(userNode, userNodeValue, new Vector2Int(x, y), nodes, terrain);
            }
        }
    }

    public override void Draw(Vector2Int userNode, GridTerrain terrain)
    {
        var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(userNode.x, userNode.y));
        Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
    }
}
