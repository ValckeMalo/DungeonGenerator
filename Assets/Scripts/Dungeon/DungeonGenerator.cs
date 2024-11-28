using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    #region Singleton
    private static DungeonGenerator instance;
    public static DungeonGenerator Instance { get => instance; }

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

    [Header("Param Dungeon")]
    [SerializeField] private Transform dungeonContainer;
    [SerializeField] private int maxDistance;
    [SerializeField] private List<Vector2Int> nextPosRoomToSpawn;
    [SerializeField] private bool isAnyRoomMaxDistance = false;

    [Header("Probability in percent to add door(s)")]
    [SerializeField] private List<ProbabilityDungeon> probabilityDungeon;

    [Header("Data")]
    [SerializeField] private SORoom dataRoom;
    [SerializeField] private Vector2Int widthDist;
    [SerializeField] private Vector2Int heightDist;
    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

    #region Get/Set
    public SORoom DataRoom { get => dataRoom; }
    public Dictionary<Vector2Int, Room> Rooms { get => rooms; }
    public Vector2Int HeightDist { get => heightDist; }
    public Vector2Int WidthDist { get => widthDist; }
    #endregion

    #region Update Dungeon Room Data
    public void UpdateDungeon(Vector2Int actualroom, Vector2Int previousRoom)
    {
        rooms[actualroom].State = Room.RoomState.Actual;
        rooms[previousRoom].State = Room.RoomState.Visited;

        UpdateNearestRoom(actualroom);
    }

    private void UpdateNearestRoom(Vector2Int actualroom)
    {
        for (int i = 0; i < rooms[actualroom].NbDoor; i++)
        {
            Vector2Int nextRoom = GetGapDirection(rooms[actualroom].DoorDirections[i]);

            if (rooms[actualroom + nextRoom].State == Room.RoomState.Hide)
            {
                rooms[actualroom + nextRoom].State = Room.RoomState.Nearest;
            }
        }
    }
    #endregion

    #region Dungeon Gen
    public void CreateDungeon()
    {
        InstantiateARoom(dataRoom.FourDoors, new Vector2Int(0, 0), 0);

        while (nextPosRoomToSpawn.Count != 0)
        {
            (List<Room.DoorDirection> doorsDirection, List<Room.DoorDirection> possibleDoors) = CountMinDoors(nextPosRoomToSpawn[0]);
            int distanceFromCenter = CalculateDistanceCenter(nextPosRoomToSpawn[0], doorsDirection);
            doorsDirection = AddAditionalDoor(doorsDirection, possibleDoors, distanceFromCenter);
            ChooseAPrefabRoom(doorsDirection.Count, doorsDirection, nextPosRoomToSpawn[0], distanceFromCenter);

            nextPosRoomToSpawn.Remove(nextPosRoomToSpawn[0]);
        }

        InitFirstRoomData();
    }

    private void InitFirstRoomData()
    {
        //player spawn always at 0,0 
        Vector2Int spawn = new Vector2Int(0, 0);

        //spawn room
        rooms[spawn].State = Room.RoomState.Actual;

        //nearest room
        rooms[spawn + widthDist].State = Room.RoomState.Nearest;
        rooms[spawn - widthDist].State = Room.RoomState.Nearest;
        rooms[spawn - heightDist].State = Room.RoomState.Nearest;
        rooms[spawn + heightDist].State = Room.RoomState.Nearest;
    }

    private int CalculateDistanceCenter(Vector2Int pos, List<Room.DoorDirection> doorsDirection)
    {
        int minDistFromCenter = int.MaxValue;
        for (int i = 0; i < doorsDirection.Count; i++)
        {
            Vector2Int gap = GetGapDirection(doorsDirection[i]);
            int currentDistCenter = rooms[pos + gap].DistanceToCenter;

            if (currentDistCenter < minDistFromCenter)
            {
                minDistFromCenter = currentDistCenter;
            }
        }

        if (minDistFromCenter++ == maxDistance)
        {
            isAnyRoomMaxDistance = true;
            if (isAnyRoomMaxDistance)
            {
                print("Max");
            }
        }


        return minDistFromCenter;
    }

    private List<Room.DoorDirection> AddAditionalDoor(List<Room.DoorDirection> dirDoors, List<Room.DoorDirection> possibleDoors, int distCenter)
    {
        if (dirDoors.Count == 4 || distCenter == maxDistance)
        {
            return dirDoors;
        }

        ProbabilityDungeon probaDungeon = probabilityDungeon.Find(proba => proba.DistanceFromCenter == distCenter);//select the good probability on the dist from the center

        if (probaDungeon.NbMaxDoorToAdd != 0 || probaDungeon.PercentAddDoor != 0)
        {
            float random = (float)Random.Range(0, 1001) / 10f; //to resetup on percent
            if (random <= probaDungeon.PercentAddDoor || (!isAnyRoomMaxDistance && nextPosRoomToSpawn.Count == 1 /* just left this room*/))
            {
                int nbRoomToAdd = 0;
                int percent = 0;

                random = (float)Random.Range(0, 1001) / 10f;
                for (int i = 0; i < probaDungeon.ListPrecentNbDoor.Count; i++)
                {
                    percent += probaDungeon.ListPrecentNbDoor[i];
                    if (random <= percent)
                    {
                        nbRoomToAdd = i + 1;
                        break;
                    }
                }

                if (possibleDoors.Count <= nbRoomToAdd)
                {
                    dirDoors.AddRange(possibleDoors);
                    return dirDoors;
                }

                if (nbRoomToAdd != 0)
                {
                    Room.DoorDirection directionInFront = (Room.DoorDirection)(((int)(dirDoors[0]) + 2) % 4);
                    if (possibleDoors.Contains(directionInFront))
                    {
                        random = Random.Range(0, 1001) / 10f;

                        if (random <= probaDungeon.PercentDoorInFront)
                        {
                            dirDoors.Add(directionInFront);
                            possibleDoors.Remove(directionInFront);
                            nbRoomToAdd--;
                        }
                    }

                    if (nbRoomToAdd != 0)
                    {
                        for (int i = 0; i < nbRoomToAdd; i++)
                        {
                            List<Room.DoorDirection> newDoorDir = possibleDoors.Where(dir => dir != directionInFront).Select(dir => dir).ToList();
                            int newRandom = Random.Range(0, newDoorDir.Count);
                            Room.DoorDirection newDir = newDoorDir[newRandom];
                            dirDoors.Add(newDir);
                            possibleDoors.Remove(newDir);
                        }
                    }
                }
            }
        }

        return dirDoors;
    }

    private void ChooseAPrefabRoom(int nbDoors, List<Room.DoorDirection> dirDoors, Vector2Int pos, int distanceFromCenter)
    {
        switch (nbDoors)
        {
            case 1:
                InstantiateARoom(ReturnOneDoorPrefab(dirDoors[0]), pos, distanceFromCenter);
                return;

            case 2:
                InstantiateARoom(ReturnTwoDoorPrefab(dirDoors), pos, distanceFromCenter);
                return;

            case 3:
                InstantiateARoom(ReturnThreeDoorPrefab(dirDoors), pos, distanceFromCenter);
                return;

            case 4:
                InstantiateARoom(dataRoom.FourDoors, pos, distanceFromCenter);
                return;

            default:
                Debug.LogError("no doors detected : " + nbDoors);
                return;
        }
    }

    private GameObject ReturnOneDoorPrefab(Room.DoorDirection dir)
    {
        return dataRoom.OneDoors.Find(room => room.Direction[0] == dir).Prefab;
    }

    private GameObject ReturnTwoDoorPrefab(List<Room.DoorDirection> dir)
    {
        return dataRoom.TwoDoors.Find(room => room.Direction.Contains(dir[0]) && room.Direction.Contains(dir[1])).Prefab;
    }

    private GameObject ReturnThreeDoorPrefab(List<Room.DoorDirection> dir)
    {
        return dataRoom.ThreeDoors.Find(room => room.Direction.Contains(dir[0]) && room.Direction.Contains(dir[1]) && room.Direction.Contains(dir[2])).Prefab;
    }

    private (List<Room.DoorDirection> doorsDirection, List<Room.DoorDirection> possibleDoors) CountMinDoors(Vector2Int pos)
    {
        List<Room.DoorDirection> dirDoors = new List<Room.DoorDirection>();
        List<Room.DoorDirection> possibleDoors = new List<Room.DoorDirection>();
        Room value;

        //north
        if (rooms.TryGetValue(pos + HeightDist, out value))//if a room exist
        {
            if (value.DoorDirections.Contains(Room.DoorDirection.South))//if a door exist
            {
                dirDoors.Add(Room.DoorDirection.North);
            }
        }
        else//just void
        {
            possibleDoors.Add(Room.DoorDirection.North);
        }

        //south
        if (rooms.TryGetValue(pos - HeightDist, out value))//if a room exist
        {
            if (value.DoorDirections.Contains(Room.DoorDirection.North))//if a door exist
            {
                dirDoors.Add(Room.DoorDirection.South);
            }
        }
        else//just void
        {
            possibleDoors.Add(Room.DoorDirection.South);
        }

        //west
        if (rooms.TryGetValue(pos - WidthDist, out value))//if a room exist
        {
            if (value.DoorDirections.Contains(Room.DoorDirection.East))//if a door exist
            {
                dirDoors.Add(Room.DoorDirection.West);
            }
        }
        else//just void
        {
            possibleDoors.Add(Room.DoorDirection.West);
        }


        //east
        if (rooms.TryGetValue(pos + WidthDist, out value))//if a room exist
        {
            if (value.DoorDirections.Contains(Room.DoorDirection.West))//if a door exist
            {
                dirDoors.Add(Room.DoorDirection.East);
            }
        }
        else//just void
        {
            possibleDoors.Add(Room.DoorDirection.East);
        }

        return (dirDoors, possibleDoors);
    }

    private void InstantiateARoom(GameObject prefab, Vector2Int pos, int distCenter)
    {
        GameObject newRoomObject = Instantiate(prefab, new Vector2(pos.x, pos.y), Quaternion.identity, dungeonContainer);
        Room newRoom = newRoomObject.GetComponent<Room>();
        newRoom.Posisiton = pos;
        newRoom.DistanceToCenter = distCenter;

        for (int i = 0; i < newRoom.NbDoor; i++)
        {
            AddNextRoomToSpawn(newRoom.DoorDirections[i], pos);
        }

        AddARoom(newRoom);
    }

    private void AddNextRoomToSpawn(Room.DoorDirection direction, Vector2Int initalPos)
    {
        switch (direction)
        {
            case Room.DoorDirection.North:
                CheckNextPosRoom(initalPos + HeightDist);
                return;

            case Room.DoorDirection.East:
                CheckNextPosRoom(initalPos + WidthDist);
                return;

            case Room.DoorDirection.South:
                CheckNextPosRoom(initalPos - HeightDist);
                return;

            case Room.DoorDirection.West:
                CheckNextPosRoom(initalPos - WidthDist);
                return;

            default:
                Debug.LogError("No dir");
                return;
        }
    }

    private void CheckNextPosRoom(Vector2Int nextPos)
    {
        if (!rooms.ContainsKey(nextPos) && !nextPosRoomToSpawn.Contains(nextPos))
        {
            nextPosRoomToSpawn.Add(nextPos);
        }
    }

    public void AddARoom(Room newRoom)
    {
        rooms.Add(newRoom.Posisiton, newRoom);
    }

    public Vector2Int GetGapDirection(Room.DoorDirection direction)
    {
        switch (direction)
        {
            case Room.DoorDirection.North:
                return heightDist;

            case Room.DoorDirection.East:
                return widthDist;

            case Room.DoorDirection.South:
                return -heightDist;

            case Room.DoorDirection.West:
                return -widthDist;

            default:
                Debug.LogError("probleme de direction");
                return widthDist;
        }
    }
    #endregion

    #region Room Type
    public void DefineTypeRoom()
    {
        BoosRoom();
        MerchantRoom();
    }

    private void BoosRoom()
    {
        List<Room> allPossibleRoom =
            rooms
            .Where(dic => dic.Value.DistanceToCenter == maxDistance && dic.Value.Type == Room.RoomType.None)
            .Select(dico => dico.Value)
            .ToList();

        allPossibleRoom[Random.Range(0, allPossibleRoom.Count)].Type = Room.RoomType.Boss;
    }

    private void MerchantRoom()
    {
        List<Room> allPossibleRoom =
            rooms
            .Where(dic => dic.Value.DistanceToCenter == maxDistance - 2 && dic.Value.Type == Room.RoomType.None)
            .Select(dico => dico.Value)
            .ToList();

        allPossibleRoom[Random.Range(0, allPossibleRoom.Count)].Type = Room.RoomType.Merchant;
    }
    #endregion
}

[System.Serializable]
public class ProbabilityDungeon
{
    [Header("Proba")]
    [SerializeField, Range(1, 5)] private int distanceFromCenter;
    [SerializeField, Range(0, 100)] private int percentAddDoor;
    [SerializeField, Range(0, 3)] private int nbMaxDoorToAdd;
    [SerializeField, Range(0, 100)] private List<int> precentNbDoor;
    [SerializeField, Range(0, 100)] private int percentDoorInFront;

    public int DistanceFromCenter { get => distanceFromCenter; }
    public int PercentAddDoor { get => percentAddDoor; }
    public int NbMaxDoorToAdd { get => nbMaxDoorToAdd; }
    public List<int> ListPrecentNbDoor { get => precentNbDoor; }
    public int PercentDoorInFront { get => percentDoorInFront; }
}