using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public class S_HealthBar : MonoBehaviour
{
    [BoxGroup("Dependencies")]
    [SerializeField] private S_GeneratorSRP _generator;
    [BoxGroup("Dependencies")]
    [SerializeField] private Slider _healthSlider;

    private S_Health _health;

    void OnEnable()
    {
        _generator.OnPlayerSpawned += Set;
    }

    void Set()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { return; }

        _health = player.GetComponentInChildren<S_Health>();
        if (_health == null) _health = player.GetComponentInParent<S_Health>();
        if (_health == null) { return; }

        _health.OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(_health.CurrentHealth);
    }

    void OnDestroy()
    {
        _health.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar(int currentHp)
    {
        float healthPercentage = (float)currentHp / _health.MaxHealth;
        _healthSlider.value = healthPercentage;
    }
}
