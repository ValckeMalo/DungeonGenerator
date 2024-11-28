using UnityEngine;
using UnityEngine.UI;

public class MiniMapGenerator : MonoBehaviour
{
    #region Singleton
    private static MiniMapGenerator instance;
    public static MiniMapGenerator Instance { get => instance; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        Init();
    }
    #endregion

    [Header("UI")]
    [SerializeField] private SOMapSprites miniMap;
    [SerializeField] private RawImage miniMapTexture;
    [SerializeField] private Color colorBack;

    private Color[] visitedColor;
    private Color[] nearestColor;
    private Color[] actualColor;

    private Color[] bossHeadColor;
    private Color[] merchantCoinColor;

    private Texture2D textureMiniMap;

    [Header("Dim MiniMap")]
    [SerializeField] int dimTextureMiniMap;
    private int centerMiniMap;
    public int DimensionTexureMiniMap { get => dimTextureMiniMap; }

    private int dimTextureRoom = 50;
    private int centerRoom;
    public int dimensionTextureRoom { get => dimTextureRoom; }

    private int dimTextureIcon = 35;
    private int centerIcon;

    private void Start()
    {
        LevelManager.Instance.onChangeRoom += UpdateMiniMap;
    }

    public void GenerateMiniMap()
    {
        InitColorBackground();
        CreateMiniMap();

        miniMapTexture.texture = textureMiniMap;
    }

    public void UpdateMiniMap(Vector2Int posRoomPlayer)
    {
        CreateMiniMap();
    }

    private void InitColorBackground()
    {
        textureMiniMap = new Texture2D(dimTextureMiniMap, dimTextureMiniMap);
        for (int i = 0; i < dimTextureMiniMap; i++)
        {
            for (int j = 0; j < dimTextureMiniMap; j++)
            {
                textureMiniMap.SetPixel(i, j, colorBack);
            }
        }
        textureMiniMap.Apply();
    }

    private void Init()
    {
        //init some variables
        centerMiniMap = dimTextureMiniMap / 2;
        centerRoom = dimTextureRoom / 2;
        centerIcon = dimTextureIcon / 2;

        //sprite room
        visitedColor = miniMap.VisitedRoom.GetPixels();
        nearestColor = miniMap.NearestRoom.GetPixels();
        actualColor = miniMap.ActualRoom.GetPixels();

        //sprites type room
        bossHeadColor = miniMap.BossSkull.GetPixels();
        merchantCoinColor = miniMap.MerchantCoin.GetPixels();
    }

    private void CreateMiniMap()
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

        textureMiniMap.Apply();
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

    private void GenerateRoomMiniMap(Color[] room, int widthGap, int heightGap)
    {
        int startX = (centerMiniMap + (widthGap * dimTextureRoom)) - centerRoom;
        int startY = (centerMiniMap + (heightGap * dimTextureRoom)) - centerRoom;

        for (int i = 0; i < dimTextureRoom; i++)
        {
            for (int j = 0; j < dimTextureRoom; j++)
            {
                textureMiniMap.SetPixel(startX + i, startY + j, room[j * dimTextureRoom + i]);
            }
        }
    }

    private void GenerateRoomIcon(Color[] iconColor, int widthGap, int heightGap)
    {
        int startX = (centerMiniMap + (widthGap * dimTextureRoom)) - centerIcon;
        int startY = (centerMiniMap + (heightGap * dimTextureRoom)) - centerIcon;

        for (int i = 0; i < dimTextureIcon; i++)
        {
            for (int j = 0; j < dimTextureIcon; j++)
            {
                if (iconColor[j * dimTextureIcon + i].a != 0)
                {
                    textureMiniMap.SetPixel(startX + i, startY + j, iconColor[j * dimTextureIcon + i]);
                }
            }
        }
    }
}