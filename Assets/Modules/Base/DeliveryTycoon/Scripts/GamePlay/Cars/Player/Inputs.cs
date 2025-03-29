using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player
{
    public class Inputs 
    {
        public Vector2 Move { get; private set; }
        
        private InputSystem_Actions _inputSystemActions;
        private InputAction _moveAction;

        public Inputs()
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Enable();
            _inputSystemActions.Player.Move.performed += OnMove;
            _inputSystemActions.Player.Move.canceled += OnMoveCanceled;
        }
        
        public void Disable() => _inputSystemActions.Disable();

        private void OnMove(CallbackContext context) => Move = context.ReadValue<Vector2>();

        private void OnMoveCanceled(CallbackContext context) => Move = Vector2.zero;
    }
}