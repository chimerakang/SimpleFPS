using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsMaster { get { return IsMaster; } }
    public PTK.Ansuz ansuzClient;
    public List<WeaponPickupSpawner> weaponsSpawners;

    public GameObject[] ChatManagerNetworkObject = null;
    public GameObject[] GameModeNetworkObject = null;
    public GameObject[] PlayerNetworkObject = null;
    public GameObject[] WeaponPickupNetworkObject = null;

    private BeardedManStudios.BMSByte metadata = new BeardedManStudios.BMSByte();
    private bool isAnsuzConnected = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        initAnszuClient();
    }

    void initAnszuClient()
    {
        if (!PTK.Ansuz.Ansuzinitialized)
        {
            ansuzClient = gameObject.AddComponent<PTK.Ansuz>();
            ansuzClient.OnConnected += arenaConnected;
            ansuzClient.OnDisconnected += arenaDisonnected;
        }

    }

    private void arenaConnected()
    {
        isAnsuzConnected = true;
        PTK.ObservableAusuz.GetArenaID()
            .Subscribe(result =>
            {
                Debug.Log("arena id:" + result.ArenaID + ",session token:" + result.SessionToken + ", isMaster:" + result.IsMaster );
                ansuzClient.SessionToken = result.SessionToken;
                ansuzClient.ArenaID = result.ArenaID;
                ansuzClient.IsMaster = result.IsMaster;
            },
                e => Debug.Log(e)
            );

        PTK.ObservableAusuz.GetAppVersion()
            .Subscribe(result =>
            {
                Debug.Log("arena version:" + result.AppVersion + ",build:" + result.Builds);
            },
                e => Debug.Log(e)
            );

    }

    private void arenaDisonnected()
    {
        isAnsuzConnected = false;
    }



}
