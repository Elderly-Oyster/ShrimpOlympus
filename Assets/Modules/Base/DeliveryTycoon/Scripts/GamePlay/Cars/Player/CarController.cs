using CodeBase.Core.Systems;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class CarController : BaseCarController
    {
        [Tooltip("Acceleration force applied to the car")]
        [SerializeField] private float acceleration = 10f;

        [Tooltip("Steering angle per second")]
        [SerializeField] private float steering = 100f;
        
        [Tooltip("Maximum car speed")]
        [SerializeField] private float maxSpeed = 20f;
        
        [Tooltip("Audio")]
        public AudioClip IdleSound;
        
        private AudioSystem _audioSystem;
        private Rigidbody _rigidbody;
        private CarInputs _carInput;
        private AudioSource _audioSource;

        [Inject]
        public void Construct(AudioSystem audioSystem, CarInputs carInput)
        {
            _carInput = carInput;
            _audioSystem = audioSystem;
            SetMusicState(_audioSystem.MusicVolume);
        }

        private void SetMusicState(float musicVolume)
        {
            if (musicVolume > 0)
            {
                _audioSource.clip = IdleSound;
                _audioSource.loop = true;
                _audioSource.playOnAwake = false;
                _audioSource.Play();
                _audioSource.volume = musicVolume - 0.6f;
            }
        }
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>(); 
            }
        }
        
        private void OnDisable() => _carInput.Disable();
        
        private void FixedUpdate()
        {
            if (_carInput != null) HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 moveInput = _carInput.Move;

            // Move forward/backward
            Vector3 moveForce = transform.forward * moveInput.y * acceleration;
            if (_rigidbody.velocity.magnitude < maxSpeed) 
                _rigidbody.AddForce(moveForce, ForceMode.Acceleration);

            // Rotate only when moving forward or backward
            if (moveInput.y != 0f && _rigidbody.velocity.magnitude > maxSpeed/10)
            {
                float turn = moveInput.x * steering * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                _rigidbody.MoveRotation(_rigidbody.rotation * turnRotation);
            }

            // Change audio pitch for movement
            _audioSource.pitch = (moveInput != Vector2.zero) ? 1.3f : 1.0f;
        }
    }
}