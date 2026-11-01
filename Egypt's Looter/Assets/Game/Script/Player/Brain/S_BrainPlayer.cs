using UnityEngine;
using System;
using UnityEngine.InputSystem;
using NaughtyAttributes;

public class S_BrainPlayer : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private S_Move _entityMovement; 

    [BoxGroup("Inputs")]
    [SerializeField] private InputActionReference _movementInput;
    [BoxGroup("Inputs")]
    [SerializeField] private InputActionReference _runInput;
    [BoxGroup("Inputs")]
    [SerializeField] private InputActionReference _jumpInput;
    [BoxGroup("Inputs")]
    [SerializeField] private InputActionReference _crounchInput;
    [BoxGroup("Inputs")]
    [SerializeField] private InputActionReference _pauseInput;

    [BoxGroup("References")]
    [SerializeField] private SO_PauseChannel _pauseChannel;


    private Vector3 _currentDirection;
    private Vector3 _jumpDirection;
    private Transform _cameraTransform;

    void Awake()
    {
        if(_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    void OnEnable()
    {
        _jumpInput.action.performed += Jump;
        _pauseInput.action.performed += Pause;
        _crounchInput.action.performed += Crounch;
    }

    void OnDisable()
    {
        _jumpInput.action.performed -= Jump;
        _pauseInput.action.performed -= Pause;
        _crounchInput.action.performed -= Crounch;
    }

    void Update()
    {
        Vector2 moveInput = _movementInput.action.ReadValue<Vector2>();
        
        if(moveInput.sqrMagnitude < 0.0001f)
        {
            _entityMovement.SetDirection(Vector3.zero);
            return;
        }

        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 moveDirection = Vector3.ClampMagnitude(forward * moveInput.y + right * moveInput.x, 1f);
        _currentDirection = moveDirection;
        bool isRunning = _runInput.action.IsPressed();
        if(isRunning)
        {
            _entityMovement.SetRunDirection(moveDirection);
        }
        else
        {
            _entityMovement.SetDirection(moveDirection);
        }
    }
    
    void Jump(InputAction.CallbackContext context)
    {
        _entityMovement.Jump();
    }

    void Pause(InputAction.CallbackContext context)
    {
        _pauseChannel.RequestPause();
    }

    void Crounch(InputAction.CallbackContext context)
    {
        _entityMovement.Crounch();
    }
}
