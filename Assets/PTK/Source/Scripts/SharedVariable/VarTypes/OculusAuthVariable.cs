using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using PTK;

/// <summary>
/// holds position, rotation, and scale
/// </summary>
[CreateAssetMenu(menuName = "PTK/Variables/Oculus Auth Variable")]
#if UNITY_EDITOR
public class OculusAuthVariable : SavableVariable, ISerializationCallbackReceiver, IListItemDrawer
#else
public class OculusAuthVariable : SavableVariable, ISerializationCallbackReceiver
#endif
{
    /// <summary>
    /// initial user email
    /// </summary>
    public string InitialEmail;
    /// <summary>
    /// initial user password
    /// </summary>
    public string InitialPassword;
    /// <summary>
    /// initial user access token
    /// </summary>
    public string InitialAccessToken;
    /// <summary>
    /// initial use standalone platform
    /// </summary>
    public bool InitialUseStandalonePlatform;


    /// <summary>
    /// runtime postion
    /// </summary>
    [System.NonSerialized]
    public string RuntimeEmail;
    /// <summary>
    /// runtime rotation
    /// </summary>
    [System.NonSerialized]
    public string RuntimePassword;
    /// <summary>
    /// runtime scale
    /// </summary>
    [System.NonSerialized]
    public string RuntimeAccessToken;
    /// <summary>
    /// runtime scale
    /// </summary>
    [System.NonSerialized]
    public bool RuntimeUseStandalonePlatform;

    /// <summary>
    /// does nothing
    /// </summary>
    /// <see cref="ISerializationCallbackReceiver"/>
    public void OnBeforeSerialize() { /* do nothing */ }

    /// <summary>
    /// apply initial values to runtime
    /// </summary>
    /// <see cref="ISerializationCallbackReceiver"/>
    public void OnAfterDeserialize()
    {
        RuntimeEmail = InitialEmail;
        RuntimePassword = InitialPassword;
        RuntimeAccessToken = InitialAccessToken;
        RuntimeUseStandalonePlatform = InitialUseStandalonePlatform;
    }

    /// <summary>
    /// deserializes string into runtime data
    /// </summary>
    /// <param name="data">serialized runtime data</param>
    public override void OnLoadData(string data)
    {
        string[] values = data.Split('|');
        // user email
        RuntimeEmail = values[0];
        // token
        RuntimeAccessToken = values[1];

        RuntimeUseStandalonePlatform = bool.Parse(values[2]);


        Loaded = true;
    }
    /// <summary>
    /// produces a serialized string of the runtime data
    /// </summary>
    /// <returns>serialized runtime data string</returns>
    public override string OnSaveData()
    {
        return RuntimeEmail + "|" + RuntimeAccessToken + "|" +
                RuntimeUseStandalonePlatform.ToString() ;
    }
    /// <summary>
    /// resets the runtime data to initial data
    /// </summary>
    public override void OnClearSave()
    {
        OnAfterDeserialize();
    }

#if UNITY_EDITOR
    /// <summary>
    /// draws the custom inspector for an element
    /// </summary>
    /// <param name="rect">size of element</param>
    /// <param name="line_height">height of a line</param>
    /// <see cref="IListItemDrawer"/>
    public void OnDrawElement(Rect rect, float line_height)
    {
        GUI.Label(new Rect(rect.x, rect.y, rect.width / 2, line_height - 2), name);
        InitialEmail = EditorGUI.TextField(new Rect(rect.x, rect.y + line_height + 6, rect.width, line_height - 2), "Default Email", InitialEmail);
        GUI.Label(new Rect(rect.x, rect.y + ((line_height + 4) * 2), rect.width, line_height - 2), "Runtime Email: " + RuntimeEmail );

        InitialPassword = EditorGUI.TextField(new Rect(rect.x, rect.y + ((line_height + 6) * 3), rect.width, line_height - 2), "Default Password", InitialPassword);

        ///InitialAccessToken = EditorGUI.TextField(new Rect(rect.x, rect.y + ((line_height + 10) * 5), rect.width, line_height - 2), "Default Token", InitialAccessToken);
        GUI.Label(new Rect(rect.x, rect.y + ((line_height + 10) * 4), rect.width, line_height - 2), "Runtime Access Token: " + RuntimeAccessToken );

        InitialUseStandalonePlatform = EditorGUI.Toggle(new Rect(rect.x, rect.y + ((line_height + 10) * 5), rect.width, line_height - 2), "Default Use Standalone", InitialUseStandalonePlatform);
        GUI.Label(new Rect(rect.x, rect.y + ((line_height +10) * 6), rect.width, line_height - 2), "Runtime Use Standalone: " + RuntimeUseStandalonePlatform );
    }

    /// <summary>
    /// returns the height of the element
    /// </summary>
    /// <param name="line_height">line height</param>
    /// <returns>height of element</returns>
    public float GetElementHeight(float line_height)
    {
        return (line_height+12) * 7;
    }
#endif

}
