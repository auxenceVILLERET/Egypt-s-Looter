using UnityEngine;
using NaughtyAttributes;

public class S_Stairs : MonoBehaviour
{
    [BoxGroup("References")]
    [SerializeField] private SO_GameManagerChannel _gameManagerChannel;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _gameManagerChannel.RequestWin();
    }
}