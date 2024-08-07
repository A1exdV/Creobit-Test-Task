using System;
using Cinemachine;
using Reflex.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

/*
  Основа взята с дефолтного Unity контроллера из пака https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526
  Из контроллера вырезаны прыжки и гравитация. управление переделано на подписки на нажатия. Character Controller заменен на Rigidbody.
  Контроллер подчиняется состоянию игры.
 */
namespace Adventure_Game.ThirdPersonController.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CustomCharacterController : MonoBehaviour
    {
        [Header("Player")] [Tooltip("Move speed of the character in m/s")] [SerializeField]
        private float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")] [SerializeField]
        private float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")] [Range(0.0f, 0.3f)] [SerializeField]
        private float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")] [SerializeField]
        private float speedChangeRate = 10.0f;

        [Tooltip("How fast the camera rotate around the character")] [SerializeField]
        private float sensitivity = 2.0f;

        [SerializeField] private AudioClip[] footstepAudioClips;
        [Range(0, 1)] [SerializeField] private float footstepAudioVolume = 0.5f;

        [Header("Cinemachine")] [SerializeField]
        private CinemachineVirtualCamera cinemachineVirtualCameraPrefab;

        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField]
        private GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")] [SerializeField]
        private float topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")] [SerializeField]
        private float bottomClamp = -30.0f;

        [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField]
        private float cameraAngleOverride;

        [Tooltip("For locking the camera position on all axis")] [SerializeField]
        private bool lockCameraPosition;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;

        private int _animIDSpeed;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private Rigidbody _rigidbody;
        private PlayerInputSystem _input;
        private GameObject _mainCamera;

        private const float Threshold = 0.01f;

        private bool _hasAnimator;

        private bool _isSprinting;
        private Vector2 _look;
        private Vector2 _move;

        public event Action OnFinishTriggerEnter;

        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            var cinemachine = Instantiate(cinemachineVirtualCameraPrefab);
            cinemachine.Follow = cinemachineCameraTarget.transform;
        }

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                     RigidbodyConstraints.FreezeRotationZ;

            _input = gameObject.scene.GetSceneContainer().Resolve<PlayerInputSystem>();


            AssignAnimationIDs();
        }

        private void Update()
        {
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
                OnFinishTriggerEnter?.Invoke();
        }

        public void EnableControl(bool enable)
        {
            if (enable)
            {
                _input.Player.Look.started += GetCameraRotation;
                _input.Player.Look.performed += GetCameraRotation;
                _input.Player.Look.canceled += GetCameraRotation;

                _input.Player.Move.started += GetDirection;
                _input.Player.Move.performed += GetDirection;
                _input.Player.Move.canceled += GetDirection;

                _input.Player.Sprint.started += Sprint;
                _input.Player.Sprint.canceled += Sprint;
            }
            else
            {
                _input.Player.Look.started -= GetCameraRotation;
                _input.Player.Look.performed -= GetCameraRotation;
                _input.Player.Look.canceled -= GetCameraRotation;

                _input.Player.Move.started -= GetDirection;
                _input.Player.Move.performed -= GetDirection;
                _input.Player.Move.canceled -= GetDirection;

                _input.Player.Sprint.started -= Sprint;
                _input.Player.Sprint.canceled -= Sprint;
                
                _move = Vector2.zero;
            }
        }

        private void Sprint(InputAction.CallbackContext callbackContext)
        {
            _isSprinting = callbackContext.ReadValue<float>() > 0 ? true : false;
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GetCameraRotation(InputAction.CallbackContext callbackContext)
        {
            _look = callbackContext.ReadValue<Vector2>() * sensitivity;
        }

        private void CameraRotation()
        {
            if (_look.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                _cinemachineTargetYaw += _look.x;
                _cinemachineTargetPitch += _look.y;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void GetDirection(InputAction.CallbackContext callbackContext)
        {
            _move = callbackContext.ReadValue<Vector2>();
        }

        private void Move()
        {
            var targetSpeed = _isSprinting ? sprintSpeed : moveSpeed;

            if (_move == Vector2.zero) targetSpeed = 0.0f;

            var currentHorizontalSpeed = new Vector3(_rigidbody.velocity.x, 0.0f, _rigidbody.velocity.z).magnitude;

            var speedOffset = 0.1f;
            var inputMagnitude = _move.normalized.magnitude;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            var inputDirection = new Vector3(_move.x, 0.0f, _move.y).normalized;

            if (_move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    rotationSmoothTime);

                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _rigidbody.velocity =
                targetDirection.normalized * (_speed) + new Vector3(0.0f, _rigidbody.velocity.y, 0.0f);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        //DON'T DELETE
        //Used by animator
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index],
                    transform.TransformPoint(_rigidbody.centerOfMass),
                    footstepAudioVolume);
            }
        }
    }
}