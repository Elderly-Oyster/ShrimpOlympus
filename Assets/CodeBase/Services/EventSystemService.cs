using UnityEngine;
using UnityEngine.EventSystems;
using VContainer.Unity;

namespace CodeBase.Services
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