using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HeightPaint : Paint
{
    private GUIContent[] _modes = {
        new GUIContent("Increment"),
        new GUIContent("Level"),
        new GUIContent("Set"),
        new GUIContent("Random")
    };
    private int _modeIndex;
    private int _setHeight;

    private int _randMinHeight = 0;
    private int _randMaxHeight = 20;
    private float _randStrength = 1;
    private Vector2 _randScale = new Vector2(0.05f, 0.05f);

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
        else if (_modeIndex == 3)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Min Height");
            _randMinHeight = EditorGUILayout.IntSlider(_randMinHeight, 0, 20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Max Height");
            _randMaxHeight = EditorGUILayout.IntSlider(_randMaxHeight, 0, 20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Strength");
            _randStrength = EditorGUILayout.Slider(_randStrength, 0, 2);
            EditorGUILayout.EndHorizontal();

            _randScale = EditorGUILayout.Vector2Field("Scale", _randScale);
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
            case 3:
                return RandomHeightAction;
            default:
                Debug.LogError("Unknown _modeIndex for HeightPaint tool: " + _modeIndex);
                return IncreaseHeightAction;
        }
    }

    private void RandomHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, GridTerrainData editCopy)
    {
        var height = Mathf.PerlinNoise(currentNode.x * _randScale.x, currentNode.y * _randScale.y) * _randStrength;
        height = (height) * (_randMaxHeight - _randMinHeight) + _randMinHeight;
        var intHeight = Mathf.Clamp(Mathf.RoundToInt(height), _randMinHeight, _randMaxHeight);
        editCopy[currentNode.x, currentNode.y] = intHeight;
    }

    private void SetHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, GridTerrainData editCopy)
    {
        editCopy[currentNode.x, currentNode.y] = _setHeight;
    }

    private static void LevelHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, GridTerrainData editCopy)
    {
        editCopy[currentNode.x, currentNode.y] = userNodeValue;
    }

    private static void IncreaseHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, GridTerrainData editCopy)
    {
        editCopy[currentNode.x, currentNode.y]++;
    }

    private static void DecreaseHeightAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, GridTerrainData editCopy)
    {
        editCopy[currentNode.x, currentNode.y]--;
    }
}
