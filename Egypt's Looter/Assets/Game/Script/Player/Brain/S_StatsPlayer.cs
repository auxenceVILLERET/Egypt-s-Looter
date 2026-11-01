using UnityEngine;
using System;
using NaughtyAttributes;

public class S_StatsPlayer : MonoBehaviour
{
    [Serializable]
    public struct JumpSettings
    {
        public float height;
        public float distance;
        public float duration;
    }

    [BoxGroup("Movement")]
    [SerializeField] private float _walkSpeed;
    [BoxGroup("Movement")]
    [SerializeField] private float _runSpeed;
    [BoxGroup("Movement")]
    [SerializeField] private float _rotationSpeed;

    [BoxGroup("Jump")]
    [SerializeField] private JumpSettings _walkJump;
    [BoxGroup("Jump")]
    [SerializeField] private JumpSettings _runJump;

    [BoxGroup("Crouch")]
    [SerializeField] private float _crouchSpeed;

    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;
    public float RotationSpeed => _rotationSpeed;
    public JumpSettings WalkJump => _walkJump;
    public JumpSettings RunJump => _runJump;
    public float CrouchSpeed => _crouchSpeed;

    void Reset()
    {
        _walkSpeed = 5f;
        _runSpeed = 10f;
        _rotationSpeed = 5f;
        _walkJump = new JumpSettings { height = 2f, distance = 3f, duration = 0.5f };
        _runJump = new JumpSettings { height = 3f, distance = 5f, duration = 0.7f };
        _crouchSpeed = 2.5f;
    }
}
