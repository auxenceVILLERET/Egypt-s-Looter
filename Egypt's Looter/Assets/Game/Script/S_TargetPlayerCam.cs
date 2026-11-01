using UnityEngine;
using Unity.Cinemachine;

public class S_TargetPlayerCam : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private S_GeneratorSRP _generator;

    void OnEnable()
    {
       _generator.OnPlayerSpawned += SetTarget;
    }

    private void SetTarget()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        _virtualCamera.Target.TrackingTarget = _player.transform;
    }
}
