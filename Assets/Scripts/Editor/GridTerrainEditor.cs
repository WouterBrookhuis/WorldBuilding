using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridTerrain))]
public class GridTerrainEditor : Editor
{
    private Brush[] _brushes = new Brush[] {
        new BasicBrush(),
        new FloodBrush(),
    };
    private bool _paintMode;
    private int _brushIndex = 0;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        if (GUILayout.Button("Reset Terrain"))
        {
            var terrain = (GridTerrain)target;
            terrain.TerrainData = new GridTerrainData(terrain.NewMapWidth, terrain.NewMapHeight, 1, 1);
            UpdateMeshes(terrain.GenerateMesh(),
                terrain.GetComponent<MeshFilter>(),
                terrain.GetComponent<MeshCollider>());
        }

        if(GUILayout.Button("Force Mesh Update"))
        {
            var terrain = (GridTerrain)target;
            UpdateMeshes(terrain.GenerateMesh(),
                terrain.GetComponent<MeshFilter>(),
                terrain.GetComponent<MeshCollider>());
        }

        _paintMode = GUILayout.Toggle(_paintMode, "Paint Mode", "Button", GUILayout.Height(50));
        var brushIcons = new GUIContent[_brushes.Length];
        for(int i = 0; i < _brushes.Length; i++)
        {
            brushIcons[i] = new GUIContent(_brushes[i].GetType().Name);
        }
        _brushIndex = GUILayout.SelectionGrid(_brushIndex, brushIcons, 4);
        GUILayout.Label("Brush Options", EditorStyles.boldLabel);
        _brushes[_brushIndex].OnInspectorGUI();

        serializedObject.ApplyModifiedProperties();
    }

    private void UpdateMeshes(Mesh mesh, MeshFilter meshFilter, MeshCollider meshCollider)
    {
        if (meshFilter != null)
        {
            var serializedMeshFilter = new SerializedObject(meshFilter);
            var prop = serializedMeshFilter.FindProperty("m_Mesh");
            prop.objectReferenceValue = mesh;
            serializedMeshFilter.ApplyModifiedProperties();
        }
        if (meshCollider != null)
        {
            var serMeshCol = new SerializedObject(meshCollider);
            var prop = serMeshCol.FindProperty("m_Mesh");
            prop.objectReferenceValue = mesh;
            serMeshCol.ApplyModifiedProperties();
        }
    }

    public static bool Raycast(GridTerrain terrain, out RaycastHit hit)
    {
        var camRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        //Debug.Log(camRay);
        var collider = terrain.GetComponent<Collider>();
        if(collider.Raycast(camRay, out hit, 10000))
        {
            //Handles.DrawLine(camRay.origin, camRay.origin + camRay.direction * hit.distance);
            return true;
        }
        return false;
    }

    private void OnSceneGUI()
    {
        if (!_paintMode)
        {
            return;
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        serializedObject.Update();
        bool needMeshUpdate = false;
        SceneView.currentDrawingSceneView.wantsMouseMove = true;
        if(Event.current.type == EventType.MouseMove)
        {
            SceneView.currentDrawingSceneView.Repaint();
        }

        var terrain = (GridTerrain)target;
        var collider = terrain.GetComponent<Collider>();
        if (!Raycast(terrain, out RaycastHit hit))
        {
            return;
        }
        var localHitPoint = terrain.transform.InverseTransformPoint(hit.point);
        var nodeCoord = terrain.TerrainData.GetClosestNode(localHitPoint);
        
        // Draw impact point for debugging
        Handles.color = Color.red;
        Handles.DrawWireCube(hit.point, Vector3.one * terrain.NodeGizmoSize);

        // Handle tool (no object changing) events
        _brushes[_brushIndex].HandleEvents(Event.current);

        // Draw actual brush points
        _brushes[_brushIndex].Draw(nodeCoord, terrain);

        // Handle editing events
        var nodes = serializedObject.FindProperty("TerrainData.Nodes");
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            if (Event.current.shift)
            {
                _brushes[_brushIndex].Apply(nodeCoord, nodes, terrain, LevelHeightAction);
            }
            else if (Event.current.control)
            {
                _brushes[_brushIndex].Apply(nodeCoord, nodes, terrain, DecreaseHeightAction);
            }
            else
            {
                _brushes[_brushIndex].Apply(nodeCoord, nodes, terrain, IncreaseHeightAction);
            }
            
            needMeshUpdate = true;
        }

        serializedObject.ApplyModifiedProperties();
        if (needMeshUpdate)
        {
            UpdateMeshes(terrain.GenerateMesh(), terrain.GetComponent<MeshFilter>(), terrain.GetComponent<MeshCollider>());
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
