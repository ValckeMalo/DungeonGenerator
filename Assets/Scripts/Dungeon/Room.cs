using Cinemachine;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum DoorDirection
    {
        North,
        East,
        South,
        West,
    }

    public enum RoomState
    {
        Visited,
        Nearest,
        Actual,
        Hide,
    }

    public enum RoomType
    {
        Boss,
        Merchant,
        Mob,
        Secret,
        Empty,
        None,
    }

    [Header("Dungeon Gen")]
    [SerializeField] private Vector2Int pos;
    [SerializeField] private int nbDoor;
    [SerializeField] private DoorDirection[] doorDir;
    [SerializeField] private int distanceToCenter = 0;

    [Header("Mini map")]
    [SerializeField] private RoomState state = RoomState.Hide;

    [Header("Cam")]
    [SerializeField] private CinemachineVirtualCamera vCam;

    [Header("Room")]
    [SerializeField] private RoomType type = RoomType.None;

    #region Get/Set
    public int DistanceToCenter { get => distanceToCenter; set => distanceToCenter = value; }
    public Vector2Int Posisiton { get => pos; set => pos = value; }
    public RoomState State { get => state; set => state = value; }
    public RoomType Type { get => type; set => type = value; }
    public DoorDirection[] DoorDirections { get => doorDir; }
    public CinemachineVirtualCamera Vcam { get => vCam; }
    public int NbDoor { get => nbDoor; }
    #endregion
}