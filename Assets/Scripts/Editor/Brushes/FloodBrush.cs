using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FloodBrush : Brush
{
    private static readonly Vector2Int TopLeft = new Vector2Int(-1, 1);
    private static readonly Vector2Int Left = new Vector2Int(-1, 0);
    private static readonly Vector2Int BottomLeft = new Vector2Int(-1, -1);
    private static readonly Vector2Int TopRight = new Vector2Int(1, 1);
    private static readonly Vector2Int Right = new Vector2Int(1, 0);
    private static readonly Vector2Int BottomRight = new Vector2Int(1, -1);
    private static readonly Vector2Int Top = new Vector2Int(0, 1);
    private static readonly Vector2Int Bottom = new Vector2Int(0, -1);

    private bool _showFloodPreview;

    public override void OnInspectorGUI()
    {
        _showFloodPreview = GUILayout.Toggle(_showFloodPreview, "Show Flood Preview (slow)");
    }

    public override void Apply(Vector2Int userNode, SerializedProperty nodes, GridTerrain terrain, NodeAction action)
    {
        var nodesToCheck = new Queue<Vector2Int>();
        var checkedNodes = new HashSet<Vector2Int>();
        nodesToCheck.Enqueue(userNode);
        int guard = 0;
        var userNodeValue = nodes.GetArrayElementAtIndex(userNode.x + userNode.y * terrain.TerrainData.Width).intValue;

        while (nodesToCheck.Count > 0 && guard < terrain.Width * terrain.Height)
        {
            var currentNode = nodesToCheck.Dequeue();
            if (checkedNodes.Contains(currentNode))
            {
                // It is possible for a node to be added multiple times to the queue
                continue;
            }
            // Apply the action to this node
            action(userNode, userNodeValue, currentNode, nodes, terrain);
            // Find neighbours that we need to add
            checkedNodes.Add(currentNode);
            Vector2Int relLeft, relTopLeft, relBottomLeft, relRight, relTopRight, relBottomRight, relTop, relBottom;
            bool left = false, right = false, top = false, bottom = false;
            if (currentNode.x >= 1)
            {
                left = true;
            }
            if (currentNode.x < terrain.Width - 1)
            {
                right = true;
            }
            if (currentNode.y >= 1)
            {
                bottom = true;
            }
            if (currentNode.y < terrain.Height - 1)
            {
                top = true;
            }
            relLeft = currentNode + Left;
            relTopLeft = currentNode + TopLeft;
            relBottomLeft = currentNode + BottomLeft;
            relRight = currentNode + Right;
            relTopRight = currentNode + TopRight;
            relBottomRight = currentNode + BottomRight;
            relTop = currentNode + Top;
            relBottom = currentNode + Bottom;

            if (left
                && !checkedNodes.Contains(relLeft)
                && nodes.GetArrayElementAtIndex(relLeft.x + relLeft.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relLeft);
            }
            if (left
                && top 
                && !checkedNodes.Contains(relTopLeft)
                && nodes.GetArrayElementAtIndex(relTopLeft.x + relTopLeft.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relTopLeft);
            }
            if (left 
                && bottom 
                && !checkedNodes.Contains(relBottomLeft)
                && nodes.GetArrayElementAtIndex(relBottomLeft.x + relBottomLeft.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relBottomLeft);
            }
            if (right
                && !checkedNodes.Contains(relRight)
                && nodes.GetArrayElementAtIndex(relRight.x + relRight.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relRight);
            }
            if (right
                && top 
                && !checkedNodes.Contains(relTopRight)
                && nodes.GetArrayElementAtIndex(relTopRight.x + relTopRight.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relTopRight);
            }
            if (right 
                && bottom
                && !checkedNodes.Contains(relBottomRight)
                && nodes.GetArrayElementAtIndex(relBottomRight.x + relBottomRight.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relBottomRight);
            }
            if (top
                && !checkedNodes.Contains(relTop)
                && nodes.GetArrayElementAtIndex(relTop.x + relTop.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relTop);
            }
            if (bottom 
                && !checkedNodes.Contains(relBottom)
                && nodes.GetArrayElementAtIndex(relBottom.x + relBottom.y * terrain.TerrainData.Width).intValue == userNodeValue)
            {
                nodesToCheck.Enqueue(relBottom);
            }
            guard++;
        }
    }

    public override void Draw(Vector2Int userNode, GridTerrain terrain)
    {
        Handles.color = Color.blue;
        if (_showFloodPreview)
        {
            var nodes = new SerializedObject(terrain).FindProperty("TerrainData.Nodes");
            Apply(userNode, nodes, terrain, DrawAction);
        }
        else
        {
            var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(userNode.x, userNode.y));
            Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
        }
    }

    private void DrawAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain)
    {
        var drawPoint = terrain.transform.TransformPoint(terrain.TerrainData.GetNodePositionUnchecked(currentNode.x, currentNode.y));
        Handles.DrawWireCube(drawPoint, Vector3.one * terrain.NodeGizmoSize);
    }
}
