using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CorridorStyle", menuName = "Egypt's Looter/SO_CorridorStyle")]
public class SO_CorridorStyle : ScriptableObject
{
    [Header("Size")]
    [SerializeField] private int _minWidth = 1;
    [SerializeField] private int _maxWidth = 3;

    [BoxGroup("Prefabs")]
    [SerializeField] private GameObject _corridorFloorPrefab;
    [BoxGroup("Prefabs")]
    [SerializeField] private GameObject _corridorFloorSpikePrefab;
    [BoxGroup("Prefabs")]
    [SerializeField] private GameObject _corridorWallPrefab;
    [BoxGroup("Prefabs")]
    [SerializeField] private GameObject _corridorWallTorchPrefab;
    [BoxGroup("Prefabs")]
    [SerializeField] private GameObject _corridorWallSpearPrefab;
    
    public GameObject[] CorridorFloorPrefabs { get { return new GameObject[] { _corridorFloorPrefab, _corridorFloorSpikePrefab }; } }
    public GameObject[] CorridorWallPrefabs { get { return new GameObject[] { _corridorWallPrefab, _corridorWallTorchPrefab, _corridorWallSpearPrefab }; } }

    public int MinWidth { get { return _minWidth; } }
    public int MaxWidth { get { return _maxWidth; } }
}
