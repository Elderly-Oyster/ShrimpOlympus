using System;
using PurrNet;
using UnityEngine;

public class CustomNetworkIdentity : NetworkBehaviour
{
    [SerializeField] private Color color;
    [SerializeField] private Renderer renderer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) SetColor(color);
    }

    [ServerRpc]
    private void SetColor(Color color1)
    {
        renderer.material.color = color1;
    }
}
