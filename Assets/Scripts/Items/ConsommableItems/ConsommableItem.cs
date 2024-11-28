using UnityEngine;

public class ConsommableItem : MonoBehaviour, IRecuperable
{
    [Header("Data")]
    [SerializeField] private EItem.ItemType type;
    [SerializeField] private Sprite sprite;

    public virtual void Recuperate()
    {
        Destroy(gameObject);
    }

    public virtual EItem.ItemType GetTypeObject()
    {
        return type;
    }
}