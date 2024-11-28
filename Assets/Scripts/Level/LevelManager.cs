using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    #region Singleton
    private static LevelManager instance;
    public static LevelManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    [Header("Player Room")]
    [SerializeField] private Vector2Int actualRoom = Vector2Int.zero;

    [Header("Change Between Scene")]
    private bool isOnHalfFade = false;
    private bool fadeFinish = false;
    public bool IsOnHalfFade { set => isOnHalfFade = value; }
    public bool FadeFinish { set => fadeFinish = value; }

    #region Delegate
    #region Dungeon
    public delegate void OnChangeRoom(Vector2Int actualRoom);
    public OnChangeRoom onChangeRoom;
    #endregion

    #region Player
    public delegate void OnTeleportPlayer(Vector2 newPos);
    public OnTeleportPlayer onTeleportPlayer;

    public delegate void OnPlayerDeath();
    public OnPlayerDeath onPlayerDeath;

    public delegate void OnLifeChanged(List<Heart> life);
    public OnLifeChanged onLifeChanged;

    public delegate void OnCollectibleChanged(int gold,int bomb,int key);
    public OnCollectibleChanged onCollectibleChanged;

    public delegate void OnActiveItemChanged(ActiveItem activeItem);
    public OnActiveItemChanged onActiveItemChanged;

    public delegate void OnPassiveItemChanged(SOPassiveItem newPassiveItem);
    public OnPassiveItemChanged onPassiveItemChanged;
    #endregion
    #endregion

    public void OnRoomChange(Room.DoorDirection direction)
    {
        //dungeon
        Vector2Int previousRoom = actualRoom;
        actualRoom += DungeonGenerator.Instance.GetGapDirection(direction);
        DungeonGenerator.Instance.UpdateDungeon(actualRoom, previousRoom);

        //invoke everyone need to change when room changed
        onChangeRoom.Invoke(actualRoom);

        //player
        StartCoroutine(OnPlayerChangeRoom(direction, actualRoom));
    }

    private IEnumerator OnPlayerChangeRoom(Room.DoorDirection commingDirection, Vector2Int actualRoom)
    {
        InputsManager.Instance.DesactivateInputs();
        FadeManager.Instance.StartFadeRoom();

        //wait for screen full black
        while (!isOnHalfFade)
        {
            yield return null;
        }

        isOnHalfFade = false;
        onTeleportPlayer.Invoke(actualRoom + GetGapInFrontOfDoor(commingDirection));

        //wait for screen whitout black
        while (!fadeFinish)
        {
            yield return null;
        }

        fadeFinish = false;
        InputsManager.Instance.ActivateInputs();
    }

    private Vector2 GetGapInFrontOfDoor(Room.DoorDirection commingDirection)
    {
        switch (commingDirection)
        {
            case Room.DoorDirection.North:
                return new Vector2(0f, -6f);

            case Room.DoorDirection.East:
                return new Vector2(-11f, 0f);

            case Room.DoorDirection.South:
                return new Vector2(0f, 6f);

            case Room.DoorDirection.West:
                return new Vector2(11f, 0f);

            default:
                Debug.LogError("no direction to spawn in front of door it's weird");
                return Vector2Int.zero;
        }
    }

    #region Dungeon
    private void Start()
    {
        InitADungeon();
    }

    private void InitADungeon()
    {
        DungeonGenerator.Instance.CreateDungeon();
        DungeonGenerator.Instance.DefineTypeRoom();

        CameraManager.Instance.ActivateFirstVCam(new Vector2Int(0, 0));
        MiniMapGenerator.Instance.GenerateMiniMap();
        MapGenerator.Instance.GenerateMap();
    }
    #endregion
}