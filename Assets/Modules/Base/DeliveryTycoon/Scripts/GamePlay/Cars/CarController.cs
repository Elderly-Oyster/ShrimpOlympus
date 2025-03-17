using CodeBase.Core.Gameplay.Cars;
using CodeBase.Core.Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    [RequireComponent(typeof(PlayerInput))]
    public class CarController : BaseCarController
    {
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        
        [Tooltip("Audio")]
        public AudioClip IdleSound;

        [SerializeField] private Camera _mainCamera;
        
        private Inputs _input;
        private float _speed;
        private CharacterController _controller;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private AudioClip _currentSound;
        private AudioSource _audioSource;

        [Inject] private AudioSystem _audioSystem;
        
        public void SetMusicState(float musicVolume)
        {
            if (musicVolume > 0)
            {
                _audioSource.clip = IdleSound;
                _audioSource.loop = true;
                _audioSource.playOnAwake = false;
                _audioSource.Play();
                _audioSource.volume = _audioSystem.MusicVolume;
            }
        }
        private void Start()
        {
            _input = GetComponent<Inputs>();
            _controller = GetComponent<CharacterController>();
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>(); 
            }
        }

        private void Update()
        {
            Move();
            // if (_audioSystem.MusicVolume != 0 && _audioSource.isPlaying == false)
            // {
            //     Debug.Log("Turning on Music");
            //     _audioSource.clip = IdleSound;
            //     _audioSource.loop = true;
            //     _audioSource.playOnAwake = false;
            //     _audioSource.Play();
            //     _audioSource.volume = _audioSystem.MusicVolume-0.6f;
            // }
        }

        private void Move()
        {
            float targetSpeed =  MoveSpeed;

            if (_input.move == Vector2.zero)
            {
                _audioSource.pitch = 1.0f;
                targetSpeed = 0.0f;
            }
            
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);
                
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                //_audioSource.pitch =1.3f;
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}