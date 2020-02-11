using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PTK;

public class AutoSave : MonoBehaviour
{
    public SettingObject[] settingObjects;
    private void OnEnable()
    {
        foreach (SettingObject settingObject in settingObjects)
            settingObject.LoadData();
    }
    private void OnDisable()
    {
        foreach (SettingObject settingObject in settingObjects)
            settingObject.SaveData();
    }
}
