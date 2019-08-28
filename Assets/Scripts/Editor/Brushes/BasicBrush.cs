using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BasicBrush : Brush
{
    private int _paintRadius;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Brush Radius");
        _paintRadius = EditorGUILayout.IntSlider(_paintRadius, 0, 20);
        EditorGUILayout.EndHorizontal();
    }

    public override void Apply(Vector2Int userNode, SerializedProperty nodes, GridTerrain terrain, NodeAction action)
    {
        var userNodeValue = nodes.GetArrayElementAtIndex(userNode.x + userNode.y * terrain.TerrainData.Width).intValue;

        for(int x = Mathf.Max(userNode.x - _paintRadius, 0); x < Mathf.Min(userNode.x + _paintRadius + 1, terrain.TerrainData.Width); x++)
        {
            for(int y = Mathf.Max(userNode.y - _paintRadius, 0); y < Mathf.Min(userNode.y + _paintRadius + 1, terrain.TerrainData.Height); y++)
            {
                action(userNode, userNodeValue, new Vector2Int(x, y), nodes, terrain);
            }
        }
    }

    public override void Draw(Vector2Int userNode, GridTerrain terrain)
    {
        Handles.color = Color.blue;
        if(_paintRadius > 0)
        {
            for(int x = Mathf.Max(userNode.x - _paintRadius, 0); x < Mathf.Min(userNode.x + _paintRadius + 1, terrain.TerrainData.Width); x++)
            {
                for(int y = Mathf.Max(userNode.y - _paintRadius, 0); y < Mathf.Min(userNode.y + _paintRadius + 1, terrain.TerrainData.Height); y++)
                {
                    var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(x, y));
                    Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
                }
            }
        }
        else
        {
            var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(userNode.x, userNode.y));
            Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
        }
    }
}
