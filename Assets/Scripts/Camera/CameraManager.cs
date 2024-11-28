using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Singleton
    private static CameraManager instance;
    public static CameraManager Instance { get => instance; }

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

    [Header("Data")]
    private Vector2Int previousRoom;

    private void Start()
    {
        LevelManager.Instance.onChangeRoom += ActivateVcam;
    }

    public void ActivateFirstVCam(Vector2Int actualRoom)
    {
        DungeonGenerator.Instance.Rooms[actualRoom].Vcam.enabled = true;
        this.previousRoom = actualRoom;
    }

    public void ActivateVcam(Vector2Int actualRoom)
    {
        DungeonGenerator.Instance.Rooms[actualRoom].Vcam.enabled = true;
        DungeonGenerator.Instance.Rooms[previousRoom].Vcam.enabled = false;
        this.previousRoom = actualRoom;
    }
}