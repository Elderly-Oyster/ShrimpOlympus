using System;
using PurrNet;
using UnityEngine;

public class CustomNetworkIdentity : NetworkBehaviour
{
    [SerializeField] private NetworkIdentity networkIdentity;
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isServer)
            return;
        
        Instantiate(networkIdentity, Vector3.zero, Quaternion.identity);
    }
    
    //One of the way to handle onSpawned
    // protected override void OnSpawned(bool asServer)
    // {
    //     base.OnSpawned(asServer);
    //
    //     if (!asServer)
    //         return;
    // }
}
