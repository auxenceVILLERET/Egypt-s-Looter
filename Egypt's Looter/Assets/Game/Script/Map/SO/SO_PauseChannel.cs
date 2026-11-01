using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_PauseChannel", menuName = "Egypt's Looter/So_PauseChannel")]
public class SO_PauseChannel : ScriptableObject
{
    public event Action OnPauseRequested;

    public void RequestPause()
    {
        OnPauseRequested?.Invoke();
    }
}