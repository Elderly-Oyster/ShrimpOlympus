using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace CodeBase.Services
{
    public class EventSystemService : IStartable
    {
        private const string EventSystemName = "EventSystem";
        public EventSystem EventSystem { get; private set; }

        public void Start()
        {
            EventSystem = CreateEventSystem();
            Object.DontDestroyOnLoad(EventSystem.gameObject);
        }

        private static EventSystem CreateEventSystem()
        {
            var eventSystem = new GameObject(EventSystemName).AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Object.DontDestroyOnLoad(eventSystem.gameObject);
            return eventSystem;
        }
        
        /* //TODO Transfer to NewInputSystem
       public class EventSystemService : IStartable
       {
           private const string EventSystemName = "EventSystem";

           public void Start()
           {
               var eventSystem = CreateEventSystem();
               Object.DontDestroyOnLoad(eventSystem.gameObject);
           }

           private static EventSystem CreateEventSystem()
           {
               var eventSystem = new GameObject(EventSystemName).AddComponent<EventSystem>();

               var inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();

               var inputActions = new InputActionAsset();
               inputModule.actionsAsset = inputActions;

               Object.DontDestroyOnLoad(eventSystem.gameObject);
               return eventSystem;
           }
       }
       */
    }
}