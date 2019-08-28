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

    public override void HandleEvents(Event e)
    {
        //if(e.type == EventType.KeyDown)
        //{
        //    switch(e.keyCode)
        //    {
        //        case KeyCode.LeftBracket:
        //            if(_paintRadius > 0)
        //            {
        //                _paintRadius--;
        //            }
        //            break;
        //        case KeyCode.RightBracket:
        //            if(_paintRadius < 64)
        //            {
        //                _paintRadius++;
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    public override void Apply(Vector2Int userNode, SerializedProperty nodes, GridTerrain terrain, NodeAction action)
    {
        // Holding control is decrease, otherwise we increase
        //int stepDelta = invert ? -1 : 1;
        var userNodeValue = nodes.GetArrayElementAtIndex(userNode.x + userNode.y * terrain.TerrainData.Width).intValue;

        for(int x = Mathf.Max(userNode.x - _paintRadius, 0); x < Mathf.Min(userNode.x + _paintRadius + 1, terrain.TerrainData.Width); x++)
        {
            for(int y = Mathf.Max(userNode.y - _paintRadius, 0); y < Mathf.Min(userNode.y + _paintRadius + 1, terrain.TerrainData.Height); y++)
            {
                //var elementProp = nodes.GetArrayElementAtIndex(x + y * terrain.TerrainData.Width);
                //if(Event.current.shift)
                //{
                //    elementProp.intValue = centerElementProp.intValue;
                //}
                //else
                //{
                //    elementProp.intValue = elementProp.intValue + stepDelta;
                //}
                action(userNode, userNodeValue, new Vector2Int(x, y), nodes, terrain);
            }
        }
    }

    public override void Draw(Vector2Int userNode, GridTerrain terrain)
    {
        // Draw actual brush points
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
