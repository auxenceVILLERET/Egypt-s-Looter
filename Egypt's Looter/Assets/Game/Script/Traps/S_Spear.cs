using UnityEngine;

public class S_Spear : MonoBehaviour
{
    [SerializeField] private S_SpearTrap _spearTrap;
    [SerializeField] private GameObject _spearPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Vector3 _modelTipAxis = Vector3.down;

    private float _spearTimer;

    void Start()
    {
        _spearTimer = _spearTrap.AttackRate;
    }

    void Update()
    {
        _spearTimer = Mathf.Min(_spearTimer + Time.deltaTime, _spearTrap.AttackRate);

        if (_spearTrap.IsActive && _spearTimer >= _spearTrap.AttackRate)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        _spearTimer = 0f;
        Transform origin = _spawnPoint != null ? _spawnPoint : transform;

        Quaternion modelOffset = Quaternion.FromToRotation(_modelTipAxis, Vector3.forward);

        GameObject spear = Instantiate(_spearPrefab, origin.position, origin.rotation * modelOffset);
        spear.GetComponent<S_SpearProjectile>().Init(
            _spearTrap.Damage, _spearTrap.SpearSpeed, _spearTrap.Range, origin.forward);
    }
}