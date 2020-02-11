using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The component for handling all the HUD and UI for the player
/// </summary>
public class HUDSystem : MonoBehaviour
{
    [Header("Script refrences")]
    [SerializeField]
    //Refrence to the network object script
    private NetworkedPlayer np;

    [SerializeField]
    //refrence to the health system
    private HealthSystem hpSystem;

    [SerializeField]
    //reference to the weapon controller
    private WeaponController wp;

    //the simple mouse look script
    [SerializeField]
    private SimpleSmoothMouseLook mouseLook;

    [Header("HUDSystem")]
    //The HP text
    [SerializeField]
    private TextMeshProUGUI HPText;
    
    //The reload circle
    [SerializeField]
    private Image reloadCircle;

    //The image for the damage overlay (a slightly transparent red image)
    [SerializeField]
    private Image damageOverlay;

    //The text appearing when a player die
    [SerializeField]
    private TextMeshProUGUI deathText;

    //the respawn counter
    [SerializeField]
    private TextMeshProUGUI respawnCounterText;
    //The coroutine for handling the respawn counter
    private Coroutine countDownCoroutine;

    //guess
    [SerializeField]
    private TextMeshProUGUI killedByText;

    //The Pause UI screen
    [SerializeField]
    private GameObject pauseUIScreen;

    //The slider for the mouse sensitivity
    [SerializeField]
    private Slider mouseSensitivitySlider;

    //The slider for the mouse smoothness
    [SerializeField]
    private Slider mouseSmoothnessSlider;

    //The corountine for the damage overlay effect
    private Coroutine damageOverlayCoroutine;

    //A corountine for the reload circle
    private Coroutine reloadCoroutine;

    // Use this for initialization
    void Start()
    {
        //Find the health system component and hook up to it's events so we can make some UI appear
        hpSystem.OnPlayerTakeDamage += UIOnPlayerTakeDamage;
        hpSystem.OnPlayerDie += UIOnPlayerDie;
        hpSystem.OnPlayerRespawn += UIOnPlayerRespawn;

        //Find the weapon controller as well and hook up events
        wp.OnReloadEvent += UIOnReload;

        //Hide the reload circle on start
        reloadCircle.fillAmount = 0;
        //and disable it
        reloadCircle.gameObject.SetActive(false);

        //disable the death text and the respawn counter, as it's not used yet
        deathText.gameObject.SetActive(false);
        respawnCounterText.gameObject.SetActive(false);

        //Hide pause UI
        pauseUIScreen.SetActive(false);

        //hide the cursor by default
        Cursor.visible = false;
    }

    /// <summary>
    /// Called from the PlayerDie event on the HealthSystem
    /// </summary>
    /// <param name="attackerName"></param>
    private void UIOnPlayerDie(string attackerName)
    {
        //only do hud if we are the owner
        if (np.networkObject.IsOwner)
        {
            deathText.gameObject.SetActive(true);
            respawnCounterText.gameObject.SetActive(true);
            //hide the HP on death as well
            HPText.gameObject.SetActive(false);
            //ensure the respawn count down corountine is stopped before starting it again
            if (countDownCoroutine != null)
            {
                StopCoroutine(countDownCoroutine);
            }
            countDownCoroutine = StartCoroutine(respawnCountdown(hpSystem.RespawnTime, hpSystem.RespawnTime));
            
            //set the killed by text
            killedByText.text = "Killed by " + attackerName;
        }
    }

    /// <summary>
    /// The coroutine handling the respawn countdown
    /// </summary>
    /// <param name="countFrom">on what the countdown should start at</param>
    /// <param name="duration">how fast the countdown should count down.... lawls</param>
    /// <returns></returns>
    IEnumerator respawnCountdown(float countFrom, float duration)
    {
        float time = duration;
        float elapsedTime = 0f;
        float countDown = countFrom;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            countDown = Mathf.Lerp(countFrom, 0, (elapsedTime / time));
            //F2 = two decimals
            respawnCounterText.text = countDown.ToString("F2");
            yield return null; //Don't freeze Unity
        }
        //when the countdown is done, hide the death text and counter
        deathText.gameObject.SetActive(false);
        respawnCounterText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called from the event on HealthSystem
    /// </summary>
    private void UIOnPlayerTakeDamage()
    {
        //only do HUD on the owner
        if (np.networkObject.IsOwner)
        {  
            //set the HP text
            string hpText = "HP: " + hpSystem.Health;
            HPText.text = hpText;

            //ensure the corountine is stopped before using it again
            if (damageOverlayCoroutine != null)
            {
                StopCoroutine(damageOverlayCoroutine);
            }
            //Do the damage fade 
            damageOverlayCoroutine = StartCoroutine(FadeImageOut(damageOverlay, 127.0f, 0.5f));
        }
        
    }

    /// <summary>
    /// Called from an event on HealthSystem on respawning
    /// </summary>
    private void UIOnPlayerRespawn()
    {
        //only do HUD on owner
        if (np.networkObject.IsOwner)
        {
            //show the hp text again on respawn and set it's value
            HPText.gameObject.SetActive(true);
            HPText.text = "HP: " + hpSystem.Health;
        }
    }

    /// <summary>
    /// called from an event on the weapon system when the player is reloading
    /// </summary>
    /// <param name="time"></param>
    private void UIOnReload(float time)
    {
        //only do HUD on the owner
        if (np.networkObject.IsOwner)
        {
            //Ensure the coroutine is stopped before using it again
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
            }
            reloadCoroutine = StartCoroutine(ProgressCircle(time));
        }
    }

    /// <summary>
    /// The corountine that handles the progess cricle from 0-1
    /// </summary>
    private IEnumerator ProgressCircle(float time)
    {
        reloadCircle.gameObject.SetActive(true);

        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            reloadCircle.fillAmount = (elapsedTime / time);
            yield return null; //Don't freeze Unity
        }
        //Hide the circle on done
        reloadCircle.fillAmount = 0;
        reloadCircle.gameObject.SetActive(false);
    }

    /// <summary>
    /// The actual corountines which lerps the alpha value
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private IEnumerator FadeImageOut(Image imageToFade, float startOpacity, float fadeTime)
    {
        var tempColor = imageToFade.color;
        tempColor.a = startOpacity / 255;
        imageToFade.color = tempColor;
        
        var t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / fadeTime;
            tempColor.a = Mathf.Lerp(tempColor.a, 0, t);
            imageToFade.color = tempColor;
            yield return null;
        }       
    }

    void Update()
    {
        if (np && np.networkObject.IsOwner)
        {
            if (Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    ShowPauseScreen();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowPauseScreen();
            }
        }
    }

    /// <summary>
    /// Called from the Update method on a keypress
    /// </summary>
    void ShowPauseScreen()
    {        
        //Show the screen
        pauseUIScreen.SetActive(true);
        //disable the mouse look
        mouseLook.enabled = false;
        //set the slider values to the mouseLook values.
        mouseSensitivitySlider.value = mouseLook.sensitivity.x;
        mouseSmoothnessSlider.value = mouseLook.smoothing.x;
        Cursor.visible = true;
    }

    /// <summary>
    /// called from the resume button on the Pause UI
    /// </summary>
    public void PauseUIResume()
    {        
        //enable the mouse look
        mouseLook.enabled = true;
        //On resume, set the mouselook's values to the slider's value
        mouseLook.sensitivity = new Vector2(mouseSensitivitySlider.value, mouseSensitivitySlider.value);
        mouseLook.smoothing = new Vector2(mouseSmoothnessSlider.value, mouseSmoothnessSlider.value);
        pauseUIScreen.SetActive(false);
        Cursor.visible = false;
    }

    /// <summary>
    /// called from the disconnect button on the Pause UI
    /// </summary>
    public void PauseUIDisconnect()
    {
        Debug.Log("Disconnect");

        np.networkObject.Networker.Disconnect(false);
        //load the main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        Cursor.visible = true;
    }

    /// <summary>
    /// Unsubscribe to all events
    /// </summary>
    private void OnDestroy()
    {
        hpSystem.OnPlayerTakeDamage -= UIOnPlayerTakeDamage;
        hpSystem.OnPlayerDie -= UIOnPlayerDie;
        hpSystem.OnPlayerRespawn -= UIOnPlayerRespawn;
    }
}
