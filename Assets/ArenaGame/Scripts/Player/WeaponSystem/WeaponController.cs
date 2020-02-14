using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The weapon controller component responsible for all shooting
/// </summary>
public class WeaponController : MonoBehaviour
{
    //An event called when the player is reloading, the float is the reload time
    public event System.Action<float> OnReloadEvent;

    [Header("Script refrences")]
    [SerializeField]
    //reference to the main networked script
    private NetworkedPlayer np;

    [SerializeField]
    //A reference to the player customization script
    private SetupPlayer setupPlayer;

    [SerializeField]
    //the camera shake component
    private CameraShake camShake;

    [Header("WeaponController")]
    //A list of all the weapons
    [SerializeField]
    private List<BaseWeapon> weaponList = new List<BaseWeapon>();

    //A list of all the view models, used for hiding them on a layer the non local player can't see
    [SerializeField]
    private GameObject[] viewModels;

    //A list of all the world models, used for hiding them on a layer the local player can't see
    [SerializeField]
    private GameObject[] worldModels;

    //The layermask the raycast shooting should use
    [SerializeField]
    private LayerMask raycastIncludeMask;

    //the view model animator
    [SerializeField]
    private Animator viewModelAnimator;

    //the world model animator
    [SerializeField]
    private Animator worldModelAnimator;

    //the world model muzzleflash object
    [SerializeField]
    private GameObject worldModelMuzzleFlash;

    //The viewmodel muzzle flash object
    [SerializeField]
    private GameObject viewModelMuzzleFlash;

    //The current weapon
    private BaseWeapon currentWeapon;

    //Is the player reloading?
    private bool isReloading = false;

    //If the weapons should auto reload when empty
    private bool autoReload = true;

    //A timer that defines when a weapon can shoot again
    private float nextShotTime;

    //A coroutine used for handling the reload enumerator
    private Coroutine reloadCoroutine;

    //the corountine to show the muzzle flash for a single frame
    private Coroutine viewModelMuzzleFlashCorountine;

    //the corountine to show the muzzle flash for a single frame
    private Coroutine worldModelMuzzleFlashCorountine;

    //the index corrisponding the weapon from the weapon list
    private int currentWeaponIndex;


    public List<BaseWeapon> WeaponList { get { return weaponList; } }
    public BaseWeapon CurrentWeapon { get { return currentWeapon; } }


    /// <summary>
    /// Called before start, setup the events from the networkplayer here, as NetworkStart is called before regular Start.
    /// Hook up all the RPC event the shoot system needs
    /// </summary>
    void Awake()
    {
        np.NetworkStartEvent += NetworkStart;
        np.ShootEvent += RPCShoot;
        np.SwitchWeaponEvent += RPCSwitchWeapon;

    }

    // Use this for initialization
    void Start()
    {
        //Set the default weapon
        currentWeapon = weaponList[0];
    }

    /// <summary>
    /// NetworkStart method called from the NetworkStartEvent on the NetworkingPlayer
    /// In NetworkStart the network object is initialized
    /// </summary>
    private void NetworkStart()
    {
        //hide the world and/or viewmodels accordingly if you are the owner or not
        //The camera only shows certain layers
        ///if (np.networkObject.IsOwner)
        if( !np.networkObject.IsRemote )
        {
            foreach (var child in worldModels)
            {
                SetLayerRecursively(child, 8);
                //hide the player model as well
                SetLayerRecursively(np.PlayerModel, 8);
            }
        }
        else
        {
            foreach (var child in viewModels)
            {
                SetLayerRecursively(child, 8);
            }
        }
    }

    /// <summary>
    /// Method that sets a layer on every child object of provided parent
    /// </summary>
    /// <param name="go">parent</param>
    /// <param name="layerNumber">layer num</param>
    void SetLayerRecursively(GameObject go, int layerNumber)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }

    /// <summary>
    /// Coroutine to show the view model muzzleflash for a single frame
    /// </summary>
    /// <param name="muzzlePos">Where the muzzleflash should be</param>
    /// <returns></returns>
    IEnumerator ShowViewModelMuzzle(Vector3 muzzlePos)
    {
        viewModelMuzzleFlash.transform.position = muzzlePos;
        viewModelMuzzleFlash.SetActive(true);
        yield return null;
        viewModelMuzzleFlash.SetActive(false);
    }

    /// <summary>
    /// Coroutine to show the world model muzzleflash for a single frame
    /// </summary>
    /// <param name="muzzlePos">Where the muzzleflash should be</param>
    /// <returns></returns>
    IEnumerator ShowWorldModelMuzzle(Vector3 muzzlePos)
    {
        worldModelMuzzleFlash.transform.position = muzzlePos;
        worldModelMuzzleFlash.SetActive(true);
        yield return null;
        worldModelMuzzleFlash.SetActive(false);
    }

    /// <summary>
    /// Called when current weapon is out of bullets
    /// </summary>
    void Reload()
    {
        isReloading = true;
        //call the event, and pass along the reload time (Used for UI)
        if (OnReloadEvent != null)
        {
            OnReloadEvent(currentWeapon.reloadTime);
        }

        //Make sure the corountine is stopped, before using it again
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
        }
        reloadCoroutine = StartCoroutine(StartReload(currentWeapon.reloadTime));

        //set the view model reload animation, only on owners as that's where the viewmodels are
        ///if (np.networkObject.IsOwner)
        if( !np.networkObject.IsRemote )
        {
            viewModelAnimator.SetTrigger("reload");
        }

    }

    /// <summary>
    /// The actual coroutine for the reload
    /// </summary>
    /// <param name="reloadTime"></param>
    /// <returns></returns>
    private IEnumerator StartReload(float reloadTime)
    {
        //Reloading takes time - wait up
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        if (currentWeapon.clips > 0)
        {
            //Decrement a clip
            currentWeapon.clips--;
            //make the bullet count be the initial clip size
            currentWeapon.bulletsPerClip = weaponList[currentWeaponIndex].bulletsPerClip;
        }
        //Make the weapon able to shoot right after reload
        nextShotTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //Don't do anything if we aren't the owner
        ///if (!np.networkObject.IsOwner)
        if( np.networkObject.IsRemote )
        {
            return;
        }
        //If the current weapon is not the hands, the 0'th index on the weapon list is the hands
        if (currentWeapon != weaponList[0])
        {
            if (Input.GetButton("Fire1") && !isReloading)
            {
                Shoot();
            }          
        }
    }

    /// <summary>
    /// The locally called shoot that calls the RPC shoot
    /// No need to always call the RPC shoot, better check some conditions first
    /// </summary>
    void Shoot()
    {
        //If the current weapon is the zero'th index, don't do anything
        if (currentWeapon != weaponList[0])
        {
            //If the CLIENT'S version have more bullets left. Note this is also getting checked on the server
            if (currentWeapon.bulletsPerClip > 0 && !isReloading)
            {
                //Wait between shots
                if (Time.time > nextShotTime)
                {
                    //set the next shot time
                    nextShotTime = Time.time + currentWeapon.timeBetweenShots;
                    //Find the points from the clients camera and send them to the server to calculate the shot from.
                    //On a fully server auth solution, the server would have a version of the camera and calculate the shot from that
                    Vector3 rayOrigin = np.PlayerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
                    Vector3 forward = np.PlayerCamera.transform.forward;
                    //send the rpc
                    np.networkObject.SendRpc(PlayerBehavior.RPC_SHOOT, BeardedManStudios.Forge.Networking.Receivers.All, rayOrigin, forward);

                    //shake that cam'
                    camShake.DoCameraShake(currentWeapon.recoilAmount, 0.1f);
                    //viewmodel shooting animation
                    viewModelAnimator.SetTrigger("shoot");

                    //Use coroutine for the muzzle flash so we can toggle it for a single frame
                    if (viewModelMuzzleFlashCorountine != null)
                    {
                        StopCoroutine(viewModelMuzzleFlashCorountine);
                    }
                    viewModelMuzzleFlashCorountine = StartCoroutine(ShowViewModelMuzzle(currentWeapon.viewModelMuzzlePoint.transform.position));
                }
            }
        }
    }

    /// <summary>
    /// The RPC for the shooting, called from the event on the networking player (called from Shoot())
    /// </summary>
    /// <param name="args"></param>
    private void RPCShoot(RpcArgs args)
    {
        //If there are bullets left in the clip and the player is not reloading
        if (currentWeapon.bulletsPerClip > 0 && !isReloading)
        {
            //Shoot!
            //Decrement the clipsize
            currentWeapon.bulletsPerClip--;

            Vector3 origin = args.GetNext<Vector3>();
            Vector3 camForward = args.GetNext<Vector3>();

            //Only make the server do the actual shooting
            if (np.networkObject.IsServer)
            {
                RaycastHit hit;
                Debug.DrawRay(origin, camForward * 10000, Color.black, 10);
                //Do the actual raycast on the server
                if (Physics.Raycast(origin, camForward, out hit, 10000, raycastIncludeMask))
                {
                    //If the current weapon doesn't use projectiles
                    if (currentWeapon.hitscan)
                    {
                        HealthSystem enemyHP = hit.collider.GetComponent<HealthSystem>();
                        if (enemyHP)
                        {
                            //call take damage and supply some raycast hit information
                            enemyHP.TakeDamage(currentWeapon.damagePerShot, setupPlayer.PlayerName, hit.point, hit.normal);
                        }
                    }
                }
            }

            //If it's not a hitscan weapon, use projectiles. Checking for collisions on the projectiles themselves is too unstable and unreliable
            //So raycast and set the raycast hit point to the target of the projectile
            if (!currentWeapon.hitscan)
            {
                RaycastHit hit;
                GameObject projectile = GameObject.Instantiate(currentWeapon.projectile, currentWeapon.worldModelMuzzlePoint.transform.position, currentWeapon.worldModelMuzzlePoint.transform.rotation);
                // Get the projectile and set the damage
                BaseProjectile p = projectile.GetComponent<BaseProjectile>();
                //Setup the damage on the projectile, but only on the server
                ///if (np.networkObject.IsServer)
                if (NetworkManager.Instance.IsMaster)
                {
                    p.damage = currentWeapon.damagePerShot;
                    p.attackerName = setupPlayer.PlayerName;
                }

                //Do the raycast and set the target on the projectile to what the raycast hit, along the hit normal (used for bloodsplatter direction)
                if (Physics.Raycast(currentWeapon.viewModelMuzzlePoint.transform.position, camForward, out hit, 10000))
                {
                    p.target = hit.point;
                    p.hitNormal = hit.normal;
                }
                else //If raycast didn't hit anything
                {
                    //just make the target in the direction, lifetime will kill this projectile
                    p.target = (currentWeapon.viewModelMuzzlePoint.transform.position + camForward) * 1000;
                    //No hit normal as it's unusual that this hits anything at all (Fired outside of map)
                }
            }

            //Play the animation
            worldModelAnimator.SetTrigger("shoot");

            //If we aren't the owner of the object (think local player), do the world model muzzleflash
            ///if (!np.networkObject.IsOwner)
            if( np.networkObject.IsRemote )
            {
                //If the corountine is already running, stop it.
                if (worldModelMuzzleFlashCorountine != null)
                {
                    StopCoroutine(worldModelMuzzleFlashCorountine);
                }
                //Start the muzzle flash corountine
                worldModelMuzzleFlashCorountine = StartCoroutine(ShowWorldModelMuzzle(currentWeapon.worldModelMuzzlePoint.transform.position));
            }
        }

        //if the clipsize is 0, autoreload is true, and we are not reloading, then, auto reload
        if (currentWeapon.bulletsPerClip == 0 && autoReload && !isReloading)
        {
            if (currentWeapon.clips <= 0)
            {
                Debug.Log("Out of ammo");
            }
            else
            {
                Reload();
            }
        }
    }

    /// <summary>
    /// The RPC for when the player gets a new weapons
    /// </summary>
    /// <param name="args"></param>
    private void RPCSwitchWeapon(RpcArgs args)
    {
        //Get the index of the weapon to switch too
        int weaponToSwitch = args.GetNext<int>();

        //set the current weapon index
        currentWeaponIndex = weaponToSwitch;
        //Hide old viewmodel & world model
        if (currentWeapon.weaponName != "Hands")
        {
            currentWeapon.viewModel.SetActive(false);
            currentWeapon.worldModel.SetActive(false);
        }

        //Switch weapon, do a clone so that the version that gets incremented ammo and such isn't the one from the list, but a copy
        currentWeapon = weaponList[weaponToSwitch].Clone();
        //if the weapon switched to, is not the hands, show the view and world models and set the animator values accordingly
        //Should probably move this to the player animator - now you got something to do! :P
        if (currentWeapon.weaponName != "Hands")
        {
            currentWeapon.viewModel.SetActive(true);
            currentWeapon.worldModel.SetActive(true);
            worldModelAnimator.SetBool("HasWeapon", true);
            //if (np.networkObject.IsOwner)
            if( !np.networkObject.IsRemote )
            {
                viewModelAnimator.SetBool("HasWeapon", true);
            }
        }
        else //The current weapon is the hands
        {
            worldModelAnimator.SetBool("HasWeapon", false);
            ///if (np.networkObject.IsOwner)
            if (!np.networkObject.IsRemote)
            {
                viewModelAnimator.SetBool("HasWeapon", false);
            }
        }

        //Set the right animation (hold with two or one hand) 
        //Could have used an animation override controller
        //Extra note: only enable the viewmodel animator on the owner
        if (currentWeapon.primary)
        {
            worldModelAnimator.SetBool("primary", true);
            ///if (np.networkObject.IsOwner)
            if (!np.networkObject.IsRemote)
            {
                viewModelAnimator.SetBool("primary", true);
            }
        }
        else
        {
            ///if (np.networkObject.IsOwner)
            if (!np.networkObject.IsRemote)
            {
                viewModelAnimator.SetBool("primary", false);
            }
            worldModelAnimator.SetBool("primary", false);
        }
    }

    /// <summary>
    /// Unsubscribe from all events on destroy
    /// </summary>
    private void OnDestroy()
    {
        np.NetworkStartEvent -= NetworkStart;
        np.ShootEvent -= RPCShoot;
        np.SwitchWeaponEvent -= RPCSwitchWeapon;
    }
}
