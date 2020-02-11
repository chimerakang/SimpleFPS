using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component responsible for handling the animator of the player's camera on death, ie the zoom in and out
/// </summary>
public class DeathCamera : MonoBehaviour
{
    //Get all the needed refrences
    [SerializeField]
    private NetworkedPlayer np;

    [SerializeField]
    private HealthSystem hp;

    [SerializeField]
    private Animator cameraAnimator;

    [SerializeField]
    private SimpleSmoothMouseLook mouseLook;

    [SerializeField]
    private WeaponController wp;

    /// <summary>
    /// Called before start, setup the events from the networkplayer here, as NetworkStart is called before regular Start.
    /// Hook up all the RPC event the shoot system needs
    /// </summary>
    void Awake()
    {
        //The SmoothMouseLook doesn't work if an animation is overriding the camera's rotation, so disable the animator untill we need it
        cameraAnimator.enabled = false;
        np.NetworkStartEvent += NetworkStart;
    }

    private void NetworkStart()
    {
        //Hook up the health systems events
        if (np.networkObject.IsOwner)
        {
            hp.OnPlayerDie += StartZoomOut;
            hp.OnPlayerRespawn += StartZoomIn;
        }
    }

    /// <summary>
    /// Called when the player dies
    /// </summary>
    /// <param name="attackerName">Not used for this</param>
    private void StartZoomOut(string attackerName)
    {
        //enable the animator
        cameraAnimator.enabled = true;
        //start the zoom out animation
        cameraAnimator.SetTrigger("zoomOut");
        //disable the mouselook
        mouseLook.enabled = false;
    }

    /// <summary>
    /// called when the player respawns
    /// </summary>
    private void StartZoomIn()
    {
        //toggle the view model
        ToggleViewModel(false);
        //enable the animator
        cameraAnimator.enabled = true;
        //start zooming in
        cameraAnimator.SetTrigger("zoomIn");
        
    }

    /// <summary>
    /// Called when the pan out animation is done (animation event)
    /// </summary>
    public void OnZoomOutDone()
    {
        Debug.Log("Zoom out done");
    }

    /// <summary>
    /// Called when the camera pan in animation is done (animation event)
    /// </summary>
    public void OnZoomInDone()
    {
        Debug.Log("Zoom in done");
        //disable the camera animator
        cameraAnimator.enabled = false;
        //enable the mouse look
        mouseLook.enabled = true;       
        //reset the mouselook position
        mouseLook._mouseAbsolute = Vector2.zero;
        //toggle the view models
        ToggleViewModel(false);      

    }

    /// <summary>
    /// Method that toggles the view model
    /// </summary>
    /// <param name="hide"></param>
    void ToggleViewModel(bool hide)
    {
        if (wp.CurrentWeapon.viewModel)
        {
            wp.CurrentWeapon.viewModel.SetActive(!hide);
        }
    }

    /// <summary>
    /// Unsubscribe from all events
    /// </summary>
    private void OnDestroy()
    {
        hp.OnPlayerDie -= StartZoomOut;
        hp.OnPlayerRespawn -= StartZoomIn;
        np.NetworkStartEvent -= NetworkStart;
    }
}
