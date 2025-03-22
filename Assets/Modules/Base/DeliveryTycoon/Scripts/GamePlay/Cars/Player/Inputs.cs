using UnityEngine;
using UnityEngine.InputSystem;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player
{
    public class Inputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;

        [Header("Movement Settings")]
        public bool analogMovement;
        
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        } 
    }
}