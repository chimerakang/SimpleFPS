using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking;
using System;

/// <summary>
/// The actual network object for a weapon pickup
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeaponPickup : WeaponPickupBehavior
{
    //The weapon index of the pickup
    private int weaponIndex;

    //the collider of the weapon pickup
    private Collider pickupCollider;

    //The weapon model used for the pickup
    private GameObject weaponModel;

    //The time before the weapon respawns
    private float weaponRespawnTime;

    //the corountine for the weapon respawn
    private Coroutine respawnWeaponCoroutine;

    // Use this for initialization
    void Start()
    {
        //Pickups are always triggers
        pickupCollider = GetComponent<Collider>();
        pickupCollider.isTrigger = true;
    }

    /// <summary>
    /// Called when the network object is ready & initialized
    /// </summary>
    protected override void NetworkStart()
    {
        base.NetworkStart();
        //Only call the setup from the server
        ///if (networkObject.IsServer)
        if (NetworkManager.Instance.IsMaster)
        {
            networkObject.SendRpc(RPC_SETUP, Receivers.AllBuffered, weaponIndex);
        }
       
    }

    /// <summary>
    /// Since all network object have to be instantiated, when this gets instantiated, this method gets called as well setting up the weapon index / respawn time
    /// </summary>
    /// <param name="weaponIndex"></param>
    public void SetWeapon(int weaponIndex, float respawnTime)
    {
        this.weaponIndex = weaponIndex;
        this.weaponRespawnTime = respawnTime;
    }

   
    /// <summary>
    /// Called when a collider triggers this objects collider
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        //Only do pickups on the server, as an RPC is called to all receivers if the player's server version enters it
        if (NetworkManager.Instance.IsMaster)
        {
            NetworkedPlayer np = col.GetComponent<NetworkedPlayer>();
            if (np)
            {
                //call the pickup RPC, and send along the respawn time as the clients are unaware of that variable
                networkObject.SendRpc(WeaponPickupBehavior.RPC_ON_PICKUP, Receivers.AllBuffered, weaponRespawnTime);
                //+1 to the weapon index, as the 0 index is the hands on the player.
                np.networkObject.SendRpc(PlayerBehavior.RPC_SWITCH_WEAPON, BeardedManStudios.Forge.Networking.Receivers.AllBuffered, weaponIndex + 1);
            }
        }
        
    }

    /// <summary>
    /// The pickup RPC
    /// </summary>
    /// <param name="args"></param>
    public override void OnPickup(RpcArgs args)
    {
        float receivedRespawnTime = args.GetNext<float>();
        pickupCollider.enabled = false;
        weaponModel.SetActive(false);
        //Start respawning weapon
        if (respawnWeaponCoroutine != null)
        {
            StopCoroutine(respawnWeaponCoroutine);
        }
        respawnWeaponCoroutine = StartCoroutine(RespawnWeapon(receivedRespawnTime));
    }

    IEnumerator RespawnWeapon(float weaponRespawnTime)
    {
        yield return new WaitForSeconds(weaponRespawnTime);
        pickupCollider.enabled = true;
        weaponModel.SetActive(true);
    }

    /// <summary>
    /// The setup rpc which shows the right weapon for the weapon pickup
    /// </summary>
    /// <param name="args"></param>
    public override void Setup(RpcArgs args)
    {
        //receive the arguent
        int weaponIndex = args.GetNext<int>();
        this.weaponIndex = weaponIndex;
        //find the corrisponding child
        weaponModel = transform.GetChild(weaponIndex).gameObject;
        //activate it
        weaponModel.SetActive(true);
    }
}
