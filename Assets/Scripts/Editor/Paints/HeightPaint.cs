using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeightPaint : Paint
{
    private GUIContent[] _modes = {
        new GUIContent("Increment"),
        new GUIContent("Level"),
        new GUIContent("Set")
    };
    private int _modeIndex;
    private int _setHeight;

    public override void OnInspectorGUI()
    {
        _modeIndex = GUILayout.SelectionGrid(_modeIndex, _modes, 4);
        if (_modeIndex == 0)
        {
            EditorGUILayout.LabelField("Hold CTRL to decrease height");
        }
        else if (_modeIndex == 2)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Height");
            _setHeight = EditorGUILayout.IntSlider(_setHeight, 0, 20);
            EditorGUILayout.EndHorizontal();
        }
    }

    public override Brush.NodeAction GetNodeAction(Event e)
    {
        switch (_modeIndex)
        {
            case 0:
                if(e.control)
                    return DecreaseHeightAction;
                return IncreaseHeightAction;
            case 1:
                return LevelHeightAction;
            case 2:
                return SetHeightAction;
            default:
                Debug.LogError("Unknown _modeIndex for HeightPaint tool: " + _modeIndex);
                return IncreaseHeightAction;
        }
    }

    private void SetHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain)
    {
        var elementProp = nodes.GetArrayElementAtIndex(currentNode.x + currentNode.y * terrain.TerrainData.Width);
        elementProp.intValue = _setHeight;
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
