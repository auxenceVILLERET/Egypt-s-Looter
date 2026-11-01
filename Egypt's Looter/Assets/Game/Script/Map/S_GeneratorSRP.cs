using UnityEngine;
using NaughtyAttributes;

public class S_GeneratorSRP : MonoBehaviour
{
    [BoxGroup("Seed")]
    [SerializeField] private int _seed;
    [BoxGroup("Seed")]
    [SerializeField] private bool _useRandomSeed;

    [BoxGroup("Player")]
    [SerializeField] private GameObject _player;

    [SerializeField] SO_DataMap _dataMap;
    [SerializeField] SO_RoomStyle _startRoom;
    [SerializeField] SO_RoomStyle _chestRoom;
    [SerializeField] SO_RoomStyle[] _roomStyles;
    [SerializeField] int _roomSpacing = 5;

    [BoxGroup("Corridors")]
    [SerializeField] SO_CorridorStyle[] _corridorsStyles;
    [BoxGroup("Corridors")]
    [Tooltip("Cases libres minimum entre une room et le L")]
    [SerializeField] int _cornerClearance = 2;
    [BoxGroup("Corridors")]
    [SerializeField] bool _allowShortLegs = true;

    [BoxGroup("Walls")]
    [Tooltip("Hauteur Y à laquelle les murs sont posés")]
    [SerializeField] float _wallY = 0f;
    [BoxGroup("Walls")]
    [Tooltip("Impose une case vide entre un couloir et toute autre room ou couloir")]
    [SerializeField] bool _requireWallGap = true;
    [BoxGroup("Walls")]
    [Tooltip("Décalage vers l'extérieur")]
    [SerializeField] float _wallOffset = 0f;
    [BoxGroup("Walls")]
    [Tooltip("Rotation ajoutée en Y")]
    [SerializeField] float _wallYawOffset = 0f;

    [BoxGroup("Props/Traps")]
    [Tooltip("Mettre piege avant props")]
    [SerializeField] SO_Rules[] _rules;
    [BoxGroup("Props/Traps")]
    [Tooltip("Ajustement manuel de la position des props")]
    [SerializeField] Vector2 _propsOffset = Vector2.zero;

    [BoxGroup("Ceiling")]
    [SerializeField] float _ceilingY = 4f;

    [BoxGroup("Grid")]
    [SerializeField] float _cellSize = 4f;

    [BoxGroup("Prefabs")]
    [SerializeField] GameObject _doorPrefabs;

    public event System.Action OnPlayerSpawned;

    S_DungeonData _data = new S_DungeonData();
    S_DungeonSpawner _spawner;
    GameObject _playerInstance;

    [ContextMenu("Generate")]
    void Generate()
    {
        Clear();
        GenerateSeed();

        S_RoomGenerator roomGenerator = new S_RoomGenerator(_data, _dataMap, _startRoom, _chestRoom, _roomStyles, _roomSpacing);
        S_CorridorGenerator corridorGenerator = new S_CorridorGenerator(_data, _dataMap, _corridorsStyles, _cornerClearance, _allowShortLegs, _requireWallGap);
        S_WallGenerator wallGenerator = new S_WallGenerator(_data, _doorPrefabs);
        _spawner = new S_DungeonSpawner(_data, transform, _cellSize, _ceilingY, _wallY, _wallOffset, _wallYawOffset, _rules, _propsOffset);

        roomGenerator.GenerateStartRoom();
        roomGenerator.GenerateRooms();
        corridorGenerator.GenerateCorridors();
        _data._endRoom = S_DungeonPathfinder.FindFarthestRoom(_data);
        _data._endRoom._style = _chestRoom;
        wallGenerator.GenerateWalls();
        wallGenerator.GenerateDoors();

        _spawner.SpawnDungeon();
        SpawnPlayer();
    }

    public void StartGame()
    {
        Generate();
    }

    void GenerateSeed()
    {
        if (_useRandomSeed)
        {
            _seed = Random.Range(int.MinValue, int.MaxValue);
        }
        Random.InitState(_seed);
    }

    void Clear()
    {
        _data.Clear();
        if (_playerInstance != null)
        {
            DestroyImmediate(_playerInstance);
            _playerInstance = null;
        }

        if (_spawner != null)
            _spawner.Clear();
    }

    void SpawnPlayer()
    {
        if (_player == null)
            return;

        S_RoomData startRoom = _data.StartRoom;
        if (startRoom == null)
            return;

        Vector3 playerPosition = startRoom.GetWorldCenter(_cellSize, 1f);
        _playerInstance = Instantiate(_player, playerPosition, Quaternion.identity);
        OnPlayerSpawned?.Invoke();
    }

    void OnDrawGizmos()
    {
        S_DungeonGizmos.DrawRoom(_data._endRoom, _cellSize, _ceilingY);
    }
}