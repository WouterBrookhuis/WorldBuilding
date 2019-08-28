using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridTerrain))]
public class GridTerrainEditor : Editor
{
    private Brush[] _brushes = new Brush[] 
    {
        new BasicBrush(),
        new FloodBrush(),
        new AllBrush(),
    };
    private Paint[] _paints = new Paint[]
    {
        new HeightPaint(),
    };
    private bool _paintMode;
    private int _brushIndex = 0;
    private int _paintIndex = 0;

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

        GUILayout.Label("Brush Options", EditorStyles.boldLabel);
        var brushIcons = new GUIContent[_brushes.Length];
        for(int i = 0; i < _brushes.Length; i++)
        {
            brushIcons[i] = new GUIContent(_brushes[i].GetType().Name);
        }
        _brushIndex = GUILayout.SelectionGrid(_brushIndex, brushIcons, 4);
        _brushes[_brushIndex].OnInspectorGUI();

        GUILayout.Label("Paint Options", EditorStyles.boldLabel);
        var paintIcons = new GUIContent[_paints.Length];
        for(int i = 0; i < _paints.Length; i++)
        {
            paintIcons[i] = new GUIContent(_paints[i].GetType().Name);
        }
        _paintIndex = GUILayout.SelectionGrid(_paintIndex, paintIcons, 4);
        _paints[_paintIndex].OnInspectorGUI();

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

        UnityEngine.Profiling.Profiler.BeginSample("GridTerrainEditor Brush Drawing");
        // Draw actual brush points
        _brushes[_brushIndex].Draw(nodeCoord, terrain);
        UnityEngine.Profiling.Profiler.EndSample();

        // Handle editing events
        var nodes = serializedObject.FindProperty("TerrainData.Nodes");
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            var action = _paints[_paintIndex].GetNodeAction(Event.current);
            UnityEngine.Profiling.Profiler.BeginSample("GridTerrainEditor Brush Application");
            _brushes[_brushIndex].Apply(nodeCoord, nodes, terrain, action);
            UnityEngine.Profiling.Profiler.EndSample();
            needMeshUpdate = true;
        }

        serializedObject.ApplyModifiedProperties();
        if (needMeshUpdate)
        {
            UpdateMeshes(terrain.GenerateMesh(), terrain.GetComponent<MeshFilter>(), terrain.GetComponent<MeshCollider>());
        }
    }

}
