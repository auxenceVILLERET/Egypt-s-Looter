using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class S_SpearProjectile : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _startPosition;
    private float _range;
    private int _damage;
    private bool _initialized;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Init(int damage, float speed, float range, Vector3 direction)
    {
        _damage = damage;
        _range = range;
        _startPosition = transform.position;
        _initialized = true;

        _rb.useGravity = false;
        _rb.linearVelocity = direction.normalized * speed;
    }

    void Update()
    {
        if (!_initialized) return;

        if ((transform.position - _startPosition).sqrMagnitude >= _range * _range)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        S_Health health = other.GetComponentInParent<S_Health>();
        if (health != null)
        {
            health.TakeDamage(_damage);
        }

        Destroy(gameObject);
    }
}