using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace PTK
{
    /// <summary>
    /// Base abstract class for Scriptable Object variables. Extend this class to create new Savable Shared Variables.
    /// </summary>
    /// <typeparam name="T">Type of runtime value</typeparam>
    /*

    public abstract class SharedVariableEditor : Editor
    {

        /// <summary>
        /// draws a custom inspector of the variable
        /// </summary>
        /// <param name="rect">size of element</param>
        /// <param name="line_height">height of a line</param>
        public void OnDrawElement(Rect rect, float line_height)
        {
            GUI.Label(new Rect(rect.position, new Vector2(rect.width, line_height)), name);
            GUI.Label(new Rect(new Vector2(rect.position.x, rect.position.y + line_height), new Vector2(rect.width, line_height)), ToString());
        }

        /// <summary>
        /// gets the height of the element
        /// </summary>
        /// <param name="line_height">used to calculate height</param>
        /// <returns>height of the element</returns>
        public virtual float GetElementHeight(float line_height)
        {
            return (line_height + 2) * 2;
        }
    }
            */

}