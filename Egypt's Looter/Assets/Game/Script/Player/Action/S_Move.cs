using UnityEngine;
using NaughtyAttributes;

public class S_Move : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private GameObject _entity;
    [BoxGroup("Dependencies")]
    [SerializeField] private Animator _animator;
    [BoxGroup("Dependencies")]
    [SerializeField] private GameObject _footPos;
    [BoxGroup("Dependencies")]
    [SerializeField] private Rigidbody _rigidbody;

    [BoxGroup("Ground Check")]
    [SerializeField] private float _groundCheckDistance = 0.1f;
    [BoxGroup("Ground Check")]
    [SerializeField] private LayerMask _groundMask = ~0;

    private Vector3 _currentDirection;
    private bool _isRunning = false;
    private bool _isGrounded = true;
    private bool _jumpRequested = false;
    private bool _isJumping = false;
    private float _jumpTimer = 0f; 
    private Vector3 _jumpHorizontalVelocity;
    private float _jumpGravity;
    private S_StatsPlayer _statsPlayer;
    private float _currentJumpDuration;

    private bool _isCrouching = false;
    private CapsuleCollider _collider;
    private float _originalColliderHeight;
    private float _crouchColliderHeight;
    private float _originalColliderCenterY;
    private float _crouchColliderCenterY;
    void Awake()
    {
        _statsPlayer = _entity.GetComponent<S_StatsPlayer>();
        _collider = _entity.GetComponentInChildren<CapsuleCollider>();
        _originalColliderHeight = _collider.height;
        _crouchColliderHeight = _originalColliderHeight / 1.5f;
        _originalColliderCenterY = _collider.center.y;
        _crouchColliderCenterY = _originalColliderCenterY - (_originalColliderHeight - _crouchColliderHeight) / 2f;

        if (_rigidbody == null)
            _rigidbody = _entity.GetComponent<Rigidbody>();

        _rigidbody.freezeRotation = true;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void SetDirection(Vector3 direction)
    {
        _currentDirection = direction;
        _isRunning = false;
    }

    public void SetRunDirection(Vector3 direction)
    {
        _currentDirection = direction;
        _isRunning = true;
    }

    public void Jump()
    {
        if (!_isGrounded || _isJumping) return;
        _jumpRequested = true;
    }

    public void Crounch()
    {
        if (!_isGrounded || _isJumping) return;

        _isCrouching = !_isCrouching;
        _animator.SetBool("IsCrouching", _isCrouching);
    }

    void Update()
    {
        UpdateCrounch();
    }

    void UpdateCrounch()
    {
        float targetHeight = _isCrouching ? _crouchColliderHeight : _originalColliderHeight;
        float targetCenterY = _isCrouching ? _crouchColliderCenterY : _originalColliderCenterY;

        float t = 1f - Mathf.Exp(-10f * Time.deltaTime);
        _collider.height = Mathf.Lerp(_collider.height, targetHeight, t);

        Vector3 center = _collider.center;
        center.y = Mathf.Lerp(center.y, targetCenterY, t);
        _collider.center = center;
    }

    void FixedUpdate()
    {
        CheckGround();

        if (_jumpRequested)
        {
            _jumpRequested = false;
            StartJump();
        }

        if (_isJumping)
            UpdateJump();
        else
            UpdateGroundMovement();
    }

    private void CheckGround()
    {
        _isGrounded = Physics.Raycast(
            _footPos.transform.position,
            Vector3.down,
            _groundCheckDistance,
            _groundMask,
            QueryTriggerInteraction.Ignore);
    }

    private void UpdateGroundMovement()
    {
        Vector3 velocity = _rigidbody.linearVelocity;

        if (_currentDirection != Vector3.zero)
        {
            float speed = _isRunning ? _statsPlayer.RunSpeed : _statsPlayer.WalkSpeed;
            if(_isCrouching)
            {
                speed = _statsPlayer.CrouchSpeed;       
            }
            _animator.SetFloat("Speed", speed / _statsPlayer.RunSpeed);

            Vector3 dir = _currentDirection.normalized;
            velocity.x = dir.x * speed;
            velocity.z = dir.z * speed;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            _rigidbody.MoveRotation(Quaternion.Slerp(
                _rigidbody.rotation,
                targetRotation,
                _statsPlayer.RotationSpeed * Time.fixedDeltaTime));
        }
        else
        {
            _animator.SetFloat("Speed", 0f);
            velocity.x = 0f;
            velocity.z = 0f;
        }

        _rigidbody.linearVelocity = velocity;
    }

    private void StartJump()
    {
        bool isMoving = _currentDirection != Vector3.zero;
        bool isRunJump = isMoving && _isRunning;
        S_StatsPlayer.JumpSettings jump = isRunJump ? _statsPlayer.RunJump : _statsPlayer.WalkJump;

        Vector3 dir = isMoving ? _currentDirection.normalized : Vector3.zero;
        float horizontalSpeed = isMoving ? jump.distance / jump.duration : 0f;
        _jumpHorizontalVelocity = dir * horizontalSpeed;

        float v0 = 4f * jump.height / jump.duration;
        _jumpGravity = 8f * jump.height / (jump.duration * jump.duration);
        _currentJumpDuration = jump.duration;

        _rigidbody.useGravity = false;
        _rigidbody.linearVelocity = new Vector3(_jumpHorizontalVelocity.x, v0, _jumpHorizontalVelocity.z);

        _jumpTimer = 0f;
        _isJumping = true;

        _animator.SetFloat("Speed", 0f);
        _animator.SetTrigger(isRunJump ? "isRunJumping" : "isJumping");
    }

    private void UpdateJump()
    {
        _jumpTimer += Time.fixedDeltaTime;

        Vector3 velocity = _rigidbody.linearVelocity;
        velocity.x = _jumpHorizontalVelocity.x;
        velocity.z = _jumpHorizontalVelocity.z;
        velocity.y -= _jumpGravity * Time.fixedDeltaTime;
        _rigidbody.linearVelocity = velocity;

        bool landed = _jumpTimer > 0.1f && velocity.y <= 0f && _isGrounded;
        bool timeout = _jumpTimer >= _currentJumpDuration + 0.25f; 

        if (landed || timeout)
        {
            EndJump();
        }
    }

    private void EndJump()
    {
        _isJumping = false;
        _rigidbody.useGravity = true; 
    }
}