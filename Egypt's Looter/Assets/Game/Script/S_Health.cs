using UnityEngine;
using NaughtyAttributes;
using System;
using System.Collections;
using Unity.Cinemachine;

public class S_Health : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private int _maxHealth = 100;
    [BoxGroup("Dependencies")]
    [SerializeField] private int _currentHealth;

    [BoxGroup("References")]
    [SerializeField] private SO_GameManagerChannel _gameManager;
    private CinemachineImpulseSource _impulseSource;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public event Action<int> OnHealthChanged;


    void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _currentHealth = _maxHealth;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnHealthChanged?.Invoke(_currentHealth);
        _impulseSource.GenerateImpulse();
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        _gameManager.RequestLose();
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        if(_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        OnHealthChanged?.Invoke(_currentHealth);
    }

}
