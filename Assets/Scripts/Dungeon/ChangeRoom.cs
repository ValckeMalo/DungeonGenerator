using UnityEngine;

public class ChangeRoom : MonoBehaviour
{
    [Header("Direction")]
    [SerializeField] private Room.DoorDirection direction;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelManager.Instance.OnRoomChange(direction);
        }
    }
}