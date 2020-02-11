using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for a weapon
/// </summary>
[System.Serializable]
public class BaseWeapon
{
    //The weapon name
    [SerializeField]
    public string weaponName;

    //The viewmodel of the weapon
    [SerializeField]
    public GameObject viewModel;

    //The worldmodel of the weapon
    [SerializeField]
    public GameObject worldModel;

    //Damage
    [SerializeField]
    public int damagePerShot;

    //Magazine count
    [SerializeField]
    public int clips;

    //The bullet count
    [SerializeField]
    public int bulletsPerClip;

    //Time before the weapon can be shot again
    [SerializeField]
    public float timeBetweenShots;

    //How long time it takes to reload
    [SerializeField]
    public float reloadTime;

    //How much recoil there should be
    [SerializeField]
    public float recoilAmount;

    //If the weapon is a primary weapon, used for animation on the world model
    [SerializeField]
    public bool primary;

    //the point where the viewmodel muzzle point should be
    [SerializeField]
    public GameObject viewModelMuzzlePoint;

    //the point where the world model muzzle point should be
    [SerializeField]
    public GameObject worldModelMuzzlePoint;

    //if it's a hitscan weapon (using raycasts)
    [SerializeField]
    public bool hitscan = true;

    //the projectile used for non hitscan weapons
    [SerializeField]
    public GameObject projectile;

    public BaseWeapon Clone()
    {
        return (BaseWeapon)this.MemberwiseClone();
    }

}
