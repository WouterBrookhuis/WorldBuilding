using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class Brush
{
    public delegate void NodeAction(Vector2Int userNode, int userNodeValue, Vector2Int currentNode, SerializedProperty nodes, GridTerrain terrain);

    /// <summary>
    /// Called before drawing to allow the brush the respond to keyboard events.
    /// Can be used for size changes etc.
    /// </summary>
    /// <param name="e"></param>
    public virtual void HandleEvents(Event e)
    {
    }

    /// <summary>
    /// Can be used to provide some custom inspector UI for a brush.
    /// </summary>
    public virtual void OnInspectorGUI()
    {
    }

    /// <summary>
    /// Draws the brush in the scene view so the user can see it.
    /// </summary>
    /// <param name="userNode"></param>
    /// <param name="terrain"></param>
    public abstract void Draw(Vector2Int userNode, GridTerrain terrain);
    /// <summary>
    /// Applies the brush to the given nodes.
    /// </summary>
    /// <param name="userNode">The node the user is clicking on</param>
    /// <param name="nodes">The serialized node array</param>
    /// <param name="terrain">The terrain object being edited, READ ONLY</param>
    /// <param name="invert">When true the brush should function inverted, so lower terrain for example</param>
    public abstract void Apply(Vector2Int userNode, SerializedProperty nodes, GridTerrain terrain, NodeAction action);
}
