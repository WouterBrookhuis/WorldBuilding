using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeightPaint : Paint
{
    public override Brush.NodeAction GetNodeAction(Event e)
    {
        if (e.shift)
        {
            return LevelHeightAction;
        }
        else if (e.control)
        {
            return DecreaseHeightAction;
        }
        else
        {
            return IncreaseHeightAction;
        }
    }

    private static void LevelHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain)
    {
        var elementProp = nodes.GetArrayElementAtIndex(currentNode.x + currentNode.y * terrain.TerrainData.Width);
        elementProp.intValue = userNodeValue;
    }

    private static void IncreaseHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain)
    {
        var elementProp = nodes.GetArrayElementAtIndex(currentNode.x + currentNode.y * terrain.TerrainData.Width);
        elementProp.intValue++;
    }

    private static void DecreaseHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain)
    {
        var elementProp = nodes.GetArrayElementAtIndex(currentNode.x + currentNode.y * terrain.TerrainData.Width);
        elementProp.intValue--;
    }
}
