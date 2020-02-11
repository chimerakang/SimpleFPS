using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The component responsible for taking damage, killing, and respawning the player
/// </summary>
public class HealthSystem : MonoBehaviour
{
    //setup events for when important stuff happenens
    public event System.Action<string> OnPlayerDie;
    public event System.Action OnPlayerTakeDamage;
    public event System.Action OnPlayerRespawn;

    [Header("Script refrences")]
    [SerializeField]
    //Reference to the network object script
    private NetworkedPlayer np;

    [SerializeField]
    //the camera shake for when hit
    private CameraShake camShake;

    [SerializeField]
    //For accesing the ragdoll
    private SetupPlayer setupPlayer;

    [Header("HealthSystem")]
    //The particle played upon death
    [SerializeField]
    private ParticleSystem deathParticle;

    //The blood impact particle
    [SerializeField]
    private ParticleSystem bloodImpactParticle;


    //The health variable and a public property for it
    private int health = 100;

    //The respawn time in seconds and a public property for it
    private float respawnTime = 3.0f;

    //The script that spawns the player, used for getting the spawnpoints
    private SpawnPlayer playerSpawn;

    //The corutine that delays the respawn
    private Coroutine respawnCoroutine;

    //so yeah you get this one
    private bool isAlive = true;

    //properties
    public int Health { get { return health; } }
    public float RespawnTime { get { return respawnTime; } }

    /// <summary>
    /// Called before start, setup the events from the networkplayer here, as NetworkStart is called before regular Start.
    /// Hook up all the RPC events the health system needs
    /// </summary>
    void Awake()
    {
        np.DieEvent += RPCDie;
        np.TakeDamageEvent += RPCTakeDamage;
    }

    // Use this for initialization
    void Start()
    {        
        //find the playerspawn from in the map
        playerSpawn = FindObjectOfType<SpawnPlayer>();
    }


    /// <summary>
    /// Only called on the server
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount, string attackerName, Vector3 hitPoint, Vector3 hitNormal)
    {
        //Don't take damage if the player already is dead
        if (!isAlive)
        {
            return;
        }
        //subtract the health
        health -= amount;
        if (health <= 0)
        {
            Debug.Log("Player died");
            //send out an RPC telling all this player died
            np.networkObject.SendRpc(PlayerBehavior.RPC_DIE, BeardedManStudios.Forge.Networking.Receivers.All, attackerName);
            isAlive = false;
        }
        //Send a RPC to the all with info about the hit
        np.networkObject.SendRpc(PlayerBehavior.RPC_TAKE_DAMAGE, BeardedManStudios.Forge.Networking.Receivers.All, health, hitPoint,hitNormal);
 
    }

    /// <summary>
    /// The RPC called when TakeDamage is called
    /// </summary>
    /// <param name="args"></param>
    private void RPCTakeDamage(RpcArgs args)
    {
        //Get the first argument
        int healthLeft = args.GetNext<int>();
        //set the health variable, as health is only subtracked from the server
        health = healthLeft;

        //call the take damage event
        if (OnPlayerTakeDamage != null)
        {
            OnPlayerTakeDamage();
        }        

        //Do a lil' camera shake for impact
        if (np.networkObject.IsOwner)
        {            
            camShake.DoCameraShake(1.0f, 0.1f);
        }

        //Get the rest of the arguments
        Vector3 hitPoint = args.GetNext<Vector3>();
        Vector3 hitNormal = args.GetNext<Vector3>();

        //Spawn the blood particle. TODO: Do some object pooling here as it's not ideal to spawn a new particle each hit
        if (bloodImpactParticle != null)
        {
            if (hitNormal == Vector3.zero || hitPoint == Vector3.zero)
            {
                return;
            }
            var spawnedBloodParticle = Instantiate(bloodImpactParticle, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitNormal));
            spawnedBloodParticle.Play();
            //Destory the particle again
            Destroy(spawnedBloodParticle, 2.0f);
        }
    }

    /// <summary>
    /// The RPC called when the player dies
    /// </summary>
    /// <param name="args"></param>
    private void RPCDie(RpcArgs args)
    {
        //get the attacker name
        string attackerName = args.GetNext<string>();
        //call an event when the player dies
        if (OnPlayerDie != null)
        {
            OnPlayerDie(attackerName);
        }

        //Play the death particle
        if (deathParticle)
        {
            deathParticle.Play();
        }

        //Ensure the respawn coroutine is stopped before using it again
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }
        respawnCoroutine = StartCoroutine(Respawn(respawnTime));

        //switch to hands on death
        np.networkObject.SendRpc(PlayerBehavior.RPC_SWITCH_WEAPON, BeardedManStudios.Forge.Networking.Receivers.AllBuffered, 0);

    }

    /// <summary>
    /// The corountine handling the respawning
    /// </summary>
    /// <param name="respawnTime"></param>
    /// <returns></returns>
    IEnumerator Respawn(float respawnTime)
    {
        //Spawn the ragdoll
        GameObject ragdoll = Instantiate(setupPlayer.Ragdoll, np.PlayerModel.transform.position, np.PlayerModel.transform.rotation);
        //Make it parent less so it doesn't follow the player
        ragdoll.transform.parent = null;
        //since it's not a parent anymore, manually set the scale
        ragdoll.transform.localScale = transform.localScale;
        ragdoll.SetActive(true);
        //Destroy the ragdoll after 10 secs
        Destroy(ragdoll, 10);
        //Disable the PlayerModel as ragdoll now is there
        np.PlayerModel.SetActive(false);
        //wait for respawn
        yield return new WaitForSeconds(respawnTime);
        //restore HP
        health = 100;
        //yay
        if (np.networkObject.IsServer)
        {
            isAlive = true;
        }

        //respawn to a spawnPosition
        if (np.networkObject.IsOwner)
        {
            //find a random spawnposition from the SpawnPlayer scripts spawnpoints
            Vector3 randomSpawnPosition = playerSpawn.SpawnPoints[UnityEngine.Random.Range(0, playerSpawn.SpawnPoints.Count-1)].position;
            transform.position = randomSpawnPosition;
            //call the respawn event
            if (OnPlayerRespawn != null)
            {
                OnPlayerRespawn();
            }           
        }
        np.networkObject.SnapInterpolations();

        //waiting a little while to ensure the object has moved to the new position before the model is turned on again
        yield return new WaitForSeconds(0.2f);
        np.PlayerModel.SetActive(true);

    }

    /// <summary>
    /// Unsubscribe to events
    /// </summary>
    private void OnDestroy()
    {
        np.DieEvent -= RPCDie;
        np.TakeDamageEvent -= RPCTakeDamage;
    }


}
