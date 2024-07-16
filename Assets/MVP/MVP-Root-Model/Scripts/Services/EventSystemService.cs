using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class EventSystemService : IStartable
    {
        public void Start()
        {
            var eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            Object.DontDestroyOnLoad(eventSystem.gameObject);
        }
    }
}