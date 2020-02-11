using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine.Events;
using BeardedManStudios.Forge.Networking;
using System;
using BeardedManStudios.Forge.Networking.Unity;

/// <summary>
/// The main network object script
/// Only thing to take notice of is that all RPC's are setup with events, so componenets can subscribe and the code will be split out across components
/// </summary>
public class NetworkedPlayer : PlayerBehavior
{

    //A toggle event, see the merry fragmas 3.0 (https://unity3d.com/learn/tutorials/topics/multiplayer-networking/merry-fragmas-30-multiplayer-fps-foundation)
    //for a proper explanation
    [System.Serializable]
    public class ToggleEvent : UnityEvent<bool> { }

    //make an event for each RPC, so other scripts can acces them 
    public event System.Action<RpcArgs> ShootEvent;
    public event System.Action<RpcArgs> DieEvent;
    public event System.Action<RpcArgs> SwitchWeaponEvent;
    public event System.Action<RpcArgs> TakeDamageEvent;
    public event System.Action<RpcArgs> SetupPlayerEvent;
    //also make an event for the network start as some components need that as well
    public event System.Action NetworkStartEvent;

    //The spine of the player's rig
    [SerializeField]
    private GameObject spine;

    //the player model
    [SerializeField]
    private GameObject playerModel;

    //Use the toggle event class
    //See the inspector for this
    [SerializeField]
    ToggleEvent ownerScripts;

    //the player's HUD canvas
    [SerializeField]
    private GameObject HUD;

    //The player's camera
    private Camera playerCamera;

    public GameObject PlayerModel
    {
        get
        {
            return playerModel;
        }
    }
    public Camera PlayerCamera
    {
        get
        {
            return playerCamera;
        }
    }

    /// <summary>
    /// Called when the network object is ready and initialized
    /// </summary>
    protected override void NetworkStart()
    {
        base.NetworkStart();

        //make a dynamic bool depending on we are the owner or not, and define the logic in the inspector.
        //eg some scripts should be disabled on non owners
        ownerScripts.Invoke(networkObject.IsOwner);

        //Get the player camera
        playerCamera = GetComponentInChildren<Camera>();


        //Disable the camera if we aren't the owner
        if (!networkObject.IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
        }
        else if (networkObject.IsOwner)
        {
            //Enable the HUD if we are the owner (it's disabled in the prefab)
            HUD.SetActive(true);
            //call the network start event
            if (NetworkStartEvent != null)
            {
                NetworkStartEvent();
            }
        }

        if (NetworkManager.Instance.Networker is IServer)
        {
            //here you can also do some server specific code
        }
        else
        {
            //setup the disconnected event
            NetworkManager.Instance.Networker.disconnected += DisconnectedFromServer;

        }
    }

    /// <summary>
    /// Called when a player disconnects
    /// </summary>
    /// <param name="sender"></param>
    private void DisconnectedFromServer(NetWorker sender)
    {
        NetworkManager.Instance.Networker.disconnected -= DisconnectedFromServer;

        MainThreadManager.Run(() =>
        {
            //Loop through the network objects to see if the disconnected player is the host
            foreach (var no in sender.NetworkObjectList)
            {
                if (no.Owner.IsHost)
                {
                    BMSLogger.Instance.Log("Server disconnected");
                    //Should probably make some kind of "You disconnected" screen. ah well
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
            }

            NetworkManager.Instance.Disconnect();
        });
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (networkObject.IsOwner)
        {
            //set the position and the rotation 
            networkObject.position = transform.position;
            networkObject.rotation = playerModel.transform.rotation;

            //Playercamera is not found the first few frames, as networkStart is a little slow
            if (spine && playerCamera)
            {
                //set the spine to the camera's rotation on one axis
                spine.transform.localEulerAngles = new Vector3(spine.transform.localEulerAngles.x, playerCamera.transform.eulerAngles.x, spine.transform.localEulerAngles.z);
                //sync it over the NCW fields
                networkObject.spineRotation = spine.transform.localEulerAngles;
            }
        }
        else //non owner, meaning a remote playe
        {
            //receive all NCW fields and use them
            transform.position = networkObject.position;
            playerModel.transform.rotation = networkObject.rotation;
            if (spine)
            {
                spine.transform.localEulerAngles = networkObject.spineRotation;
            }
        }

    }

    /// <summary>
    /// The RPC for shooting, calls an event
    /// </summary>
    /// <param name="args"></param>
    public override void Shoot(RpcArgs args)
    {
        if (ShootEvent != null)
        {
            ShootEvent(args);
        }
    }

    /// <summary>
    /// RPC for dying, calls an event
    /// </summary>
    /// <param name="args"></param>
    public override void Die(RpcArgs args)
    {
        if (DieEvent != null)
        {
            DieEvent(args);
        }
    }

    /// <summary>
    /// RPC for switching weapons, calls an event
    /// </summary>
    /// <param name="args"></param>
    public override void SwitchWeapon(RpcArgs args)
    {
        if (SwitchWeaponEvent != null)
        {
            SwitchWeaponEvent(args);
        }
    }

    /// <summary>
    /// The RPC for taking damage, calls an event
    /// </summary>
    /// <param name="args"></param>
    public override void TakeDamage(RpcArgs args)
    {
        if (TakeDamageEvent != null)
        {
            TakeDamageEvent(args);
        }
    }

    /// <summary>
    /// The RPC for setting up the player (skin, playername), calls an event
    /// </summary>
    /// <param name="args"></param>
    public override void SetupPlayer(RpcArgs args)
    {
        if (SetupPlayerEvent != null)
        {
            SetupPlayerEvent(args);
        }
    }
}
