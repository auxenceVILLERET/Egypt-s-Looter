using UnityEngine;
using UnityEngine.Playables;

public class S_ChestTimeline : MonoBehaviour
{
    [SerializeField] PlayableDirector _timeline;
    [SerializeField] ParticleSystem _particle;
    [SerializeField] SO_GameManagerChannel _gameManagerChannel;
    bool _isPlay = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_isPlay)
            return;

        _timeline.Play();
        _isPlay = true;
        _gameManagerChannel.NotifyChestOpened();
    }

    public void ChestOpen()
    {
        _particle.Play();
    }
}