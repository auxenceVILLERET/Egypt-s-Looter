using UnityEngine;

public class S_Spikes : MonoBehaviour
{
    [SerializeField] private S_SpikesTrap _spikesTrap;


    private float _spikesTimer;

    void Update()
    {
        _spikesTimer += Time.deltaTime;
        if (_spikesTimer >= _spikesTrap.SpikesDelayForStart && _spikesTrap.SpikesActiveWithTrigger == false)
        {
            SpikesMove();
        }
    }

    void SpikesMove()
    {
        float spikesPosition = Mathf.PingPong(Time.time * _spikesTrap.SpikesSpeed, _spikesTrap.SpikesHeight - _spikesTrap.SpikesMinHeight) + _spikesTrap.SpikesMinHeight;
        transform.localPosition = new Vector3(transform.localPosition.x, spikesPosition, transform.localPosition.z);
    }

    void OnTriggerEnter(Collider other)
    {
        S_Health health = other.GetComponentInParent<S_Health>();
        if (health != null)
        {
            health.TakeDamage(_spikesTrap.SpikesDamage);
        }
    }
}
