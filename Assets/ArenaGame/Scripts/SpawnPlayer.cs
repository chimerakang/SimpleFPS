using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class responsible for spawning the player on a random spawnpoint
/// </summary>
public class SpawnPlayer : MonoBehaviour
{
    private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints { get { return spawnPoints; } }
    PTK.ArenaObservable.SpawnData _spawnData = new PTK.ArenaObservable.SpawnData();

    // Use this for initialization
    void Start()
    {        
        //get all the children
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints.Add(transform.GetChild(i));
            //remove the physical appearence of the spawnpoints
            transform.GetChild(i).gameObject.SetActive(false);
        }
        //Find a random spawnpoint from the list
        Vector3 randomSpawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)].position;
        //Instantiate the player
        NetworkManager.Instance.InstantiatePlayer(position: randomSpawnPosition);        
    }

}
