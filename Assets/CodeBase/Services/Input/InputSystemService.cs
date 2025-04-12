using System;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace CodeBase.Services
{
    /// <summary>
    /// A service responsible for creating and managing the InputSystem_Actions instance and controlling Action Maps.
    /// </summary>
    public class InputSystemService : IStartable, IDisposable
    {
        private const string EventSystemObjectName = "EventSystem";

        public InputSystem_Actions InputActions { get; private set; }
        private EventSystem _eventSystem;

        public event Action OnSwitchToUI;
        public event Action OnSwitchToPlayerHumanoid;

        public void Start()
        {
            InitializeEventSystem();
            InitializeInputActions();
        }

        /// <summary>
        /// Enables only the UI Action Map, disabling all others.
        /// </summary>
        public void SwitchToUI()
        {
            InputActions.PlayerCar.Disable();
            InputActions.UI.Enable();
            Debug.Log("Switched to UI mode.");
            OnSwitchToUI?.Invoke();
        }

        /// <summary>
        /// Enables PlayerHumanoid Action Map, disabling VehicleControls and UI.
        /// </summary>
        public void SwitchToPlayerCar()
        {
            InputActions.PlayerCar.Enable();
            InputActions.UI.Disable();
            Debug.Log("Switched to PlayerHumanoid mode.");
            OnSwitchToPlayerHumanoid?.Invoke();
        }

        /// <summary>
        /// Enables the UI Action Map without affecting other Action Maps.
        /// </summary>
        public void EnableUI()
        {
            InputActions.UI.Enable();
            Debug.Log("UI Action Map enabled.");
        }

        /// <summary>
        /// Disables the UI Action Map.
        /// </summary>
        public void DisableUI()
        {
            InputActions.UI.Disable();
            Debug.Log("UI Action Map disabled.");
        }

        /// <summary>
        /// Checks if the UI Action Map is enabled.
        /// </summary>
        public bool IsUIInputEnabled() => InputActions.UI.enabled;

        /// <summary>
        /// Checks if the PlayerHumanoid Action Map is enabled.
        /// </summary>
        public bool IsPlayerInputEnabled() => InputActions.PlayerCar.enabled;

        /// <summary>
        /// Sets the first selected object for UI navigation.
        /// </summary>
        /// <param name="selectedObject">The object to be set as the first selected.</param>
        public void SetFirstSelectedObject(Selectable selectedObject)
        {
            if (_eventSystem == null)
            {
                Debug.LogWarning("EventSystem is not initialized. Cannot set first selected object.");
                return;
            }

            if (selectedObject == null)
            {
                Debug.LogWarning("Selected object is null. Cannot set first selected object.");
                return;
            }

            _eventSystem.SetSelectedGameObject(selectedObject.gameObject);
            Debug.Log($"First selected object set to: {selectedObject.name}");
        }
        
        public string GetFullActionPath(InputAction action)
        {
            if (action == null)
            {
                Debug.LogWarning("InputAction is null. Cannot get full path.");
                return string.Empty;
            }

            string mapName = action.actionMap?.name ?? "UnknownMap";
            string actionName = action.name;
            return $"{mapName}/{actionName}";
        }

        public Observable<Unit> GetPerformedObservable(InputAction action)
        {
            if (action == null)
            {
                Debug.LogWarning("InputAction is null. Cannot create Observable.");
                return Observable.Empty<Unit>(); // Возвращаем пустой Observable в случае ошибки
            }

            return Observable.FromEvent(
                (Action<InputAction.CallbackContext> h) => action.performed += h,
                h => action.performed -= h
            ).Select(_ => Unit.Default); // Преобразуем в Unit для унификации
        }
        
        public void Dispose()
        {
            if (InputActions == null) return;

            InputActions.UI.Disable();
            InputActions.PlayerCar.Disable();
            
            InputActions.Disable();
            InputActions.Dispose();
        }

        /// <summary>
        /// Initializes the EventSystem, creating a new one if it doesn't exist.
        /// </summary>
        private void InitializeEventSystem()
        {
            _eventSystem = Object.FindObjectOfType<EventSystem>();
            if (_eventSystem == null)
            {
                _eventSystem = CreateEventSystem();
                Object.DontDestroyOnLoad(_eventSystem.gameObject);
                Debug.Log("Created new EventSystem.");
            }
            else
            {
                Debug.Log("Found existing EventSystem.");
            }
        }

        /// <summary>
        /// Initializes the InputSystem_Actions.
        /// </summary>
        private void InitializeInputActions()
        {
            InputActions = new InputSystem_Actions();
        }

        /// <summary>
        /// Creates a new EventSystem with an InputSystemUIInputModule.
        /// </summary>
        /// <returns>The created EventSystem.</returns>
        private static EventSystem CreateEventSystem()
        {
            var eventSystem = new GameObject(EventSystemObjectName).AddComponent<EventSystem>();
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            return eventSystem;
        }
    }
}