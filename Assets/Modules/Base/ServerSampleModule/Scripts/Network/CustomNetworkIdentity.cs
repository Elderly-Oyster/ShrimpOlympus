using System;
using PurrNet;
using UnityEngine;

public class CustomNetworkIdentity : NetworkBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Renderer renderer;

    [SerializeField] private SyncVar<int> health = new(100);
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SetColor(color);
        if (Input.GetKeyDown(KeyCode.S)) TakeDamage(10);
    }

    // [ObserversRpc(bufferLast: true)] to make new identities update 
    [ObserversRpc]
    private void SetColor(Color color1)
    {   
        renderer.material.color = color1;
    }

    [ServerRpc]
    private void TakeDamage(int damage)
    {
        health.value -= damage;
    }
}
