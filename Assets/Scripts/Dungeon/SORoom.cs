using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room",menuName = "Data Room",order = 0)]
public class SORoom : ScriptableObject
{
    [Header("All Room")]
    [Header("   Fourth Door")]
    [SerializeField] private GameObject fourDoors;
    public GameObject FourDoors { get => fourDoors; }

    [Header("   Three Door")]
    [SerializeField] private List<RoomDoors> threeDoors;
    public List<RoomDoors> ThreeDoors { get => threeDoors; }

    [Header("   Two Door")]
    [SerializeField] private List<RoomDoors> twoDoors;
    public List<RoomDoors> TwoDoors { get => twoDoors; }

    [Header("   Two Door")]
    [SerializeField] private List<RoomDoors> oneDoors;
    public List<RoomDoors> OneDoors { get => oneDoors; }
}

[System.Serializable]
public class RoomDoors
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private List<Room.DoorDirection> direction;

    public GameObject Prefab { get => prefab; }
    public List<Room.DoorDirection> Direction { get => direction; }
}