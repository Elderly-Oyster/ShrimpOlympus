using System;
using System.Collections.Generic;
using CodeBase.Core.Systems.PopupHub;
using VContainer;

namespace CodeBase.Systems.InputSystem
{
    public class InputSystem : IDisposable
    {
        private readonly InputSystem_Actions _inputActions;
        private readonly IPopupHub _popupHub;
        private readonly IObjectResolver _objectResolver;
        private readonly Stack<IEscapeListener> _escapeListeners = new();

        public InputSystem(IPopupHub popupHub)
        {
            _popupHub = popupHub;
            
            _inputActions = new InputSystem_Actions();
            _inputActions.UI.Cancel.canceled += _ => OnEscapeClicked();
            _inputActions.UI.OpenSettings.canceled += _ => OnOpenSettingsClicked();

            _inputActions.UI.Enable();
        }

        public void AddEscapeListener(IEscapeListener listener) => _escapeListeners.Push(listener);

        private void OnEscapeClicked()
        {
            var listener = _escapeListeners.Pop();
            listener.OnEscapePressed();
        }

        private void OnOpenSettingsClicked() => _popupHub.OpenSettingsPopup();

        public void Dispose()
        {
            _inputActions.UI.Disable(); // Always disable what you enabled
            _inputActions.Dispose();    // Dispose to clean up native resources
        }
    }
}