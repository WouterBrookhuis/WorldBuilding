using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Paint
{
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

    public abstract Brush.NodeAction GetNodeAction(Event e);
}
