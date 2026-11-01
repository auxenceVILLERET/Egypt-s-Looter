using UnityEngine;
using NaughtyAttributes;

public class S_SpikesTrap : MonoBehaviour
{

    [BoxGroup("Settings")]
    [SerializeField] private float _spikesSpeed = 1f;
    [BoxGroup("Settings")]
    [SerializeField] private float _spikesHeight = 1f;
    [BoxGroup("Settings")]
    [SerializeField] private float _spikesMinHeight = -5f;
    [BoxGroup("Settings")]
    [SerializeField] private int _spikesDamage = 1;
    [BoxGroup("Activation")]
    [SerializeField] private float _spikesDelayForStart = 1f;
    [BoxGroup("Activation")]
    [SerializeField] private bool _spikesActiveWithTrigger = false;

    public float SpikesSpeed => _spikesSpeed;
    public float SpikesHeight => _spikesHeight;
    public float SpikesMinHeight => _spikesMinHeight;
    public int SpikesDamage => _spikesDamage;
    public float SpikesDelayForStart => _spikesDelayForStart;
    public bool SpikesActiveWithTrigger => _spikesActiveWithTrigger;
    

    void OnTriggerEnter(Collider other)
    {
        S_Health health = other.GetComponentInParent<S_Health>();
        if (health == null)
        {
            return;
        }
        
        if (_spikesActiveWithTrigger)
        {
            _spikesActiveWithTrigger = false;
        }
    }
    
}
