using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;

/// <summary>
/// Since network objects can't be placed in a scene and have to be instatiated, use this dummy spawner
/// </summary>
public class WeaponPickupSpawner : MonoBehaviour
{
    /// <summary>
    /// The list of possible weapons, ordered by the index from the weaponList on the player's weapon controller.
    /// </summary>
    enum Weapons
    {
        Sniper = 0, RocketLauncher, Uzi, Pistol, MachineGun
    }

    //Which weapon the pickup should spawn
    [SerializeField]
    private Weapons weaponToSpawn;

    //the respawn time on the weapon
    [SerializeField]
    private float weaponRespawnTime = 7.0f;
    
    //The cube model for helping placing the weapon pickup in the editor
    [SerializeField]
    private GameObject dummyObject;

    // Use this for initialization
    void Start()
    {
        //hide the cube
        dummyObject.SetActive(false);
        
        //make the server own the pickup
        if (NetworkManager.Instance.IsMaster)
        {
            //Spawn the network object with the dummy spawner's position
            var weaponPickup = NetworkManager.Instance.InstantiateWeaponPickup(position: transform.position);
            //setup the values
            weaponPickup.GetComponent<WeaponPickup>().SetWeapon((int)weaponToSpawn, weaponRespawnTime);
        }
    }

}
