using Unity.VisualScripting;
using UnityEngine;

namespace Core.Views
{
    public class RootCanvas : MonoBehaviour
    {
        [field: SerializeField] public Transform PopupParent { get; private set; }
    }
}