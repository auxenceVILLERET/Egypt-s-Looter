using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_GameManagerChanel", menuName = "Egypt's Looter/SO_GameManagerChanel")]
public class SO_GameManagerChannel : ScriptableObject
{
    public event Action OnDiedRequested;
    public event Action OnWinRequested;
    public event Action OnChestOpened;

    public void RequestLose()
    {
        OnDiedRequested?.Invoke();
    }

    public void RequestWin()
    {
        OnWinRequested?.Invoke();
    }
    public void NotifyChestOpened()
    {
        OnChestOpened?.Invoke();
    }
}
