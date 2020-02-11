using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using PTK;
/// <summary>
/// Scriptable Ojbect for storing an int
/// </summary>
[CreateAssetMenu(menuName = "PTK/Variables/Int Variable")]
public class IntVariable : SharedVariable<int>
{
    /// <summary>
    /// deserializes string into runtime data
    /// </summary>
    /// <param name="data">serialized runtime data</param>
    public override void OnLoadData(string data)
    {
        RuntimeValue = int.Parse(data);
        Loaded = true;
    }

#if UNITY_EDITOR
    /// <summary>
    /// draws the custom inspector for an element
    /// </summary>
    /// <param name="rect">size of element</param>
    /// <param name="line_height">height of a line</param>
    /// <see cref="IListItemDrawer"/>
    public override void OnDrawElement(Rect rect, float line_height)
    {
        //base.OnDrawElement(rect, line_height);
        InitialValue = EditorGUI.IntField(new Rect(rect.position, new Vector2(rect.width, line_height - 2)), name, InitialValue);
        GUI.Label(new Rect(rect.x, rect.y + line_height, rect.width, line_height - 2), "Runtime: " + RuntimeValue.ToString() + " | Default: " + InitialValue.ToString());
    }
#endif
}
