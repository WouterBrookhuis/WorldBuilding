using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainEditorWindow : EditorWindow
{
    private bool _paintMode = false;
    private Vector2 cellSize = new Vector2(2f, 2f);

    [MenuItem("Window/Grid Terrain Editor")]
    private static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TerrainEditorWindow));
    }

    private void OnGUI()
    {
        _paintMode = GUILayout.Toggle(_paintMode, "Toggle Paint Mode");
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (_paintMode)
        {
            sceneView.wantsMouseMove = true;
            DisplaySceneOverlay();
            if (Event.current.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }
        }
    }

    private void DisplaySceneOverlay()
    {
        // Get the mouse position in world space such as z = 0
        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 mousePosition = guiRay.origin - guiRay.direction * (guiRay.origin.z / guiRay.direction.z);

        // Get the corresponding cell on our virtual grid
        Vector2Int cell = new Vector2Int(Mathf.RoundToInt(mousePosition.x / cellSize.x), Mathf.RoundToInt(mousePosition.y / cellSize.y));
        Vector2 cellCenter = cell * cellSize;

        // Vertices of our square
        Vector3 topLeft = cellCenter + Vector2.left * cellSize * 0.5f + Vector2.up * cellSize * 0.5f;
        Vector3 topRight = cellCenter - Vector2.left * cellSize * 0.5f + Vector2.up * cellSize * 0.5f;
        Vector3 bottomLeft = cellCenter + Vector2.left * cellSize * 0.5f - Vector2.up * cellSize * 0.5f;
        Vector3 bottomRight = cellCenter - Vector2.left * cellSize * 0.5f - Vector2.up * cellSize * 0.5f;

        // Rendering
        Handles.color = Color.green;
        Vector3[] lines = { topLeft, topRight, topRight, bottomRight, bottomRight, bottomLeft, bottomLeft, topLeft };
        Handles.DrawLines(lines);
    }

    void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }
}
