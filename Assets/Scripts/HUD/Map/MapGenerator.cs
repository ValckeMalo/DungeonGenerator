using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    #region Singleton
    private static MapGenerator instance;
    public static MapGenerator Instance { get => instance; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitVariable();
    }
    #endregion

    [Header("Render")]
    [SerializeField] private SOMapSprites mapSprites;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Color backgroundColor;
    private Texture2D map;

    [Header("Dim map")]
    private Vector2Int dimTextureMap = new Vector2Int(Screen.width * 4, Screen.height * 4);
    private Vector2Int centerMap;

    private Vector2Int dimTextureRoom = new Vector2Int(100, 75);
    private Vector2Int centerRoom;

    private int dimTextureIcon = 70;
    private int centerIcon;

    [Header("Room")]
    private Color[] visitedColor;
    private Color[] nearestColor;
    private Color[] actualColor;

    [Header("Icon")]
    private Color[] bossHeadColor;
    private Color[] merchantCoinColor;

    #region Init
    private void Start()
    {
        CloseMap();

        LevelManager.Instance.onChangeRoom += UpdateMap;

        map = new Texture2D(dimTextureMap.x, dimTextureMap.y);
        rawImage.texture = map;
    }

    private void InitVariable()
    {
        //init some variables
        centerMap = new Vector2Int(dimTextureMap.x / 2, dimTextureMap.y / 2);
        centerRoom = new Vector2Int(dimTextureRoom.x / 2, dimTextureRoom.y / 2);
        centerIcon = dimTextureIcon / 2;

        //sprite room
        visitedColor = mapSprites.VisitedRoom.GetPixels();
        nearestColor = mapSprites.NearestRoom.GetPixels();
        actualColor = mapSprites.ActualRoom.GetPixels();

        //icon 
        bossHeadColor = mapSprites.BossSkull.GetPixels();
        merchantCoinColor = mapSprites.MerchantCoin.GetPixels();
    }
    #endregion

    #region Call Fct
    public void OpenMap()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CloseMap()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void GenerateMap()
    {
        InitColorBackground();
        DrawMap();

        map.Apply();
    }

    private void UpdateMap(Vector2Int actualRoom)
    {
        DrawMap();

        map.Apply();
    }
    #endregion

    #region Main
    private void DrawMap()
    {
        foreach (var room in DungeonGenerator.Instance.Rooms)
        {
            int gapX = (int)room.Key.x / DungeonGenerator.Instance.WidthDist.x;
            int gapY = (int)room.Key.y / DungeonGenerator.Instance.HeightDist.y;

            if (room.Value.State == Room.RoomState.Actual)
            {
                GenerateRoomMiniMap(actualColor, gapX, gapY);
            }
            else if (room.Value.State == Room.RoomState.Visited)
            {
                GenerateRoomMiniMap(visitedColor, gapX, gapY);
                Drawicon(room.Value.Type, gapX, gapY);
            }
            else if (room.Value.State == Room.RoomState.Nearest)
            {
                GenerateRoomMiniMap(nearestColor, gapX, gapY);
                Drawicon(room.Value.Type, gapX, gapY);
            }
        }
    }

    private void Drawicon(Room.RoomType iconType, int gapX, int gapY)
    {
        if (iconType != Room.RoomType.Empty || iconType != Room.RoomType.None)
        {
            if (iconType == Room.RoomType.Boss)
            {
                GenerateRoomIcon(bossHeadColor, gapX, gapY);
            }
            else if (iconType == Room.RoomType.Merchant)
            {
                GenerateRoomIcon(merchantCoinColor, gapX, gapY);
            }
        }
    }
    #endregion

    #region Room
    private void InitColorBackground()
    {
        for (int i = 0; i < dimTextureMap.x; i++)
        {
            for (int j = 0; j < dimTextureMap.y; j++)
            {
                map.SetPixel(i, j, backgroundColor);
            }
        }
    }

    private void GenerateRoomMiniMap(Color[] room, int widthGap, int heightGap)
    {
        int startX = (centerMap.x + (widthGap * dimTextureRoom.x)) - centerRoom.x;
        int startY = (centerMap.y + (heightGap * dimTextureRoom.y)) - centerRoom.y;

        for (int i = 0; i < dimTextureRoom.x; i++)
        {
            for (int j = 0; j < dimTextureRoom.y; j++)
            {
                map.SetPixel(startX + i, startY + j, room[j * dimTextureRoom.x + i]);
            }
        }
    }
    #endregion

    #region Icon
    private void GenerateRoomIcon(Color[] iconColor, int widthGap, int heightGap)
    {
        int startX = (centerMap.x + (widthGap * dimTextureRoom.x)) - centerIcon;
        int startY = (centerMap.y + (heightGap * dimTextureRoom.y)) - centerIcon;

        for (int i = 0; i < dimTextureIcon; i++)
        {
            for (int j = 0; j < dimTextureIcon; j++)
            {
                if (iconColor[j * dimTextureIcon + i].a != 0)
                {
                    map.SetPixel(startX + i, startY + j, iconColor[j * dimTextureIcon + i]);
                }
            }
        }
    }
    #endregion
}