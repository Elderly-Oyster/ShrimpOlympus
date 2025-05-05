using CodeBase.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using static UnityEngine.InputSystem.InputAction;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player
{
    public class CarInputs 
    {
        public Vector2 Move { get; private set; }
        
        private readonly InputSystem_Actions _inputSystemActions;
        private InputAction _moveAction;
        private readonly InputSystemService _inputSystemService;
        
        public CarInputs(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
            _inputSystemActions = _inputSystemService.InputActions;
            _inputSystemActions.PlayerCar.Move.performed += OnMove;
            _inputSystemActions.PlayerCar.Move.canceled += OnMoveCanceled;
        }
        
        public void Disable() => _inputSystemActions.Disable();

        private void OnMove(CallbackContext context) => Move = context.ReadValue<Vector2>();

        private void OnMoveCanceled(CallbackContext context) => Move = Vector2.zero;
    }
}