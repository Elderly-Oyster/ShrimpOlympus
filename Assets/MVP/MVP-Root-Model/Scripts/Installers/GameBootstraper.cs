using UnityEngine;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class GameBootstraper : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
