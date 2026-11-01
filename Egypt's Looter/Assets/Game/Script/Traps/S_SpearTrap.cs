using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class S_SpearTrap : MonoBehaviour
{
    [BoxGroup("Settings")]
    [SerializeField] private int _damage = 10;
    [BoxGroup("Settings")]
    [SerializeField] private float _range = 5f;
    [BoxGroup("Settings")]
    [SerializeField] private float _attackRate = 1f;
    [BoxGroup("Settings")]
    [SerializeField] private float _spearSpeed = 10f;

    public int Damage => _damage;
    public float Range => _range;
    public float AttackRate => _attackRate;
    public float SpearSpeed => _spearSpeed;

    private readonly HashSet<S_Health> _targetsInZone = new HashSet<S_Health>();

    public bool IsActive
    {
        get
        {
            _targetsInZone.RemoveWhere(h => h == null);
            return _targetsInZone.Count > 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        S_Health health = other.GetComponentInParent<S_Health>();
        if (health != null) _targetsInZone.Add(health);
    }

    void OnTriggerExit(Collider other)
    {
        S_Health health = other.GetComponentInParent<S_Health>();
        if (health != null) _targetsInZone.Remove(health);
    }
}