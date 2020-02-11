using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// The player customization script on the main menu
/// </summary>
public class PlayerCustomization : MonoBehaviour
{
    //The left arrow button for skin selecting
    [SerializeField]
    private Button leftArrowButton;

    //The left arrow button for skin selecting
    [SerializeField]
    private Button rightArrowButton;

    //The skinned meshes of the playermodel that's placed on the main menu (For previewing skins)
    [SerializeField]
    private SkinnedMeshRenderer[] playerModel;

    //An array of all possible player skin materials
    [SerializeField]
    private Material[] playerSkins;
    //the player name field
    [SerializeField]
    private TextMeshProUGUI playerNameText;

    //the current selected skin index
    private int currentSkinIndex = 0;


    // Use this for initialization
    private void Start()
    {
        //set up the button clicks
        leftArrowButton.onClick.AddListener(LeftArrowPressed);
        rightArrowButton.onClick.AddListener(RightArrowPressed);
        //setup a method to get called when a scene changes
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        //Load the saved player skin
        var loadedPlayerSkin = PlayerPrefs.GetInt("PlayerSkinIndex",0);
        currentSkinIndex = loadedPlayerSkin;
        //Update model with loaded skin
        UpdateSkin(currentSkinIndex);

        //load player name, default to "Player Name"
        var loadedPlayerName = PlayerPrefs.GetString("PlayerName","Player Name");

        //set the player name text
        playerNameText.text = loadedPlayerName;
        
    }

    /// <summary>
    /// Called when a scene changes
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        //Make a player name if the field is left blank
        if (string.IsNullOrEmpty(playerNameText.text.Trim()))
        {
            playerNameText.text = "Player " + UnityEngine.Random.Range(0, 999);
        }

        //store the name in playerprefs
        PlayerPrefs.SetString("PlayerName", playerNameText.text);
        //and save all the player prefs
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Called when the left arrow button is pressed
    /// </summary>
    private void LeftArrowPressed()
    {
        //cycle through the skins
        if (currentSkinIndex > 0)
        {
            currentSkinIndex--;
        }
        else
        {
            currentSkinIndex = playerSkins.Length - 1;
        }
        //Update the model
        UpdateSkin(currentSkinIndex);
    }

    /// <summary>
    /// Called when the right arrow button is pressed
    /// </summary>
    private void RightArrowPressed()
    {
        //cycle through the skins
        if (currentSkinIndex >= playerSkins.Length - 1)
        {
            currentSkinIndex = 0;
        }
        else
        {
            currentSkinIndex++;
        }
        //update the skin on the model
        UpdateSkin(currentSkinIndex);
    }

    /// <summary>
    /// Updates the skin on the model
    /// </summary>
    /// <param name="skinIndex"></param>
    void UpdateSkin(int skinIndex)
    {
        //Loop through each skinned mesh and set its material
        foreach (var mesh in playerModel)
        {
            mesh.material = playerSkins[skinIndex];
        }
        //store the selected skin in player prefs
        PlayerPrefs.SetInt("PlayerSkinIndex", skinIndex);
    }

    /// <summary>
    /// unsubscribe from all
    /// </summary>
    private void OnDestroy()
    {
        leftArrowButton.onClick.RemoveListener(LeftArrowPressed);
        rightArrowButton.onClick.RemoveListener(RightArrowPressed);
        SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
    }

}
