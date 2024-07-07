using UnityEngine;

public class GameBootstraper : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
