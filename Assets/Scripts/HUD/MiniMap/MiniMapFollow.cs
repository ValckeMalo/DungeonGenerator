using UnityEngine;
using UnityEngine.UI;

public class MiniMapFollow : MonoBehaviour
{
    #region Singleton
    private static MiniMapFollow instance;
    public static MiniMapFollow Instance { get => instance; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    [Header("Render Texture")]
    [SerializeField] private RawImage miniMap;
    [SerializeField] private float zoomWidth = 0.4f;
    [SerializeField] private float zoomHeight = 0.4f;
    private float gapXY;

    private void Start()
    {
        miniMap.uvRect = new Rect(0.3f,0.3f, zoomWidth, zoomHeight);
        gapXY = (1f / (float)MiniMapGenerator.Instance.DimensionTexureMiniMap) * (float)MiniMapGenerator.Instance.dimensionTextureRoom;

        LevelManager.Instance.onChangeRoom += UpdateCenterMiniMap;
    }

    public void UpdateCenterMiniMap(Vector2Int actualRoom)
    {
        int gapX = actualRoom.x / DungeonGenerator.Instance.WidthDist.x;
        int gapY = actualRoom.y / DungeonGenerator.Instance.HeightDist.y;

        miniMap.uvRect = new Rect(0.3f + gapXY * gapX, 0.3f + gapXY * gapY, zoomWidth, zoomHeight);
    }
}