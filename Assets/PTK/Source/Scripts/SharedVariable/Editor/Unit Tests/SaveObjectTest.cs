using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace PTK.Tests
{
    /// <summary>
    /// test fixture for save object
    /// </summary>
    [TestFixture]
    public class SaveObjectTest
    {
        SettingObject settingObject = ScriptableObject.CreateInstance<SettingObject>();
        IntVariable testIntData = ScriptableObject.CreateInstance<IntVariable>();
        SavePlayerPrefs playerPrefs = ScriptableObject.CreateInstance<SavePlayerPrefs>();
        const int testData = 5;
        const int testDefault = 10;

        [UnityTest]
        public IEnumerator _1_Setup()
        {
            testIntData.name = "TestData";
            testIntData.InitialValue = testDefault;
            testIntData.RuntimeValue = testData;
            settingObject.Data = new List<SavableVariable>();
            settingObject.Data.Add(testIntData);
            settingObject.SaveLocations = new List<SaveMethod>();
            settingObject.SaveLocations.Add(playerPrefs);

            yield return null;

            Assert.NotNull(settingObject);
            Assert.NotNull(testIntData);
            Assert.NotNull(playerPrefs);
            Assert.AreEqual(testDefault, testIntData.InitialValue);
            Assert.AreEqual(testData, testIntData.RuntimeValue);
            Assert.Contains(testIntData, settingObject.Data);
            Assert.Contains(playerPrefs, settingObject.SaveLocations);

        }

        [UnityTest]
        public IEnumerator _2_Save()
        {
            settingObject.SaveData();

            yield return null;

            Assert.IsTrue(settingObject.HasSave);
            // blank out the data to simulate that it's been unloaded
            testIntData.OnAfterDeserialize();
            Assert.AreEqual(testIntData.RuntimeValue, testIntData.InitialValue);
            Assert.AreNotEqual(testData, testIntData.RuntimeValue);
        }

        [UnityTest]
        public IEnumerator _3_Load()
        {
            settingObject.LoadData();

            yield return null;

            Assert.AreEqual(testData, testIntData.RuntimeValue);
        }

        [UnityTest]
        public IEnumerator _4_Clear()
        {

            settingObject.ClearSaveData();

            yield return null;

            Assert.False(settingObject.HasSave);
        }
    }

}
