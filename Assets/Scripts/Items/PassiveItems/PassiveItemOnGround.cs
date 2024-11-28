using UnityEngine;
using UnityEngine.UI;

public class PassiveItemOnGround : MonoBehaviour, IRecuperable, IDisplayable
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private Image iconPassiveItem;

    [Header("Sprite In Scene")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Passive Item")]
    [SerializeField] private SOPassiveItem passiveItem;
    [SerializeField] private EItem.ItemType type;
    public SOPassiveItem PassiveItem { get => passiveItem; }

    #region IRecuperable
    public EItem.ItemType GetTypeObject()
    {
        return type;
    }

    public void Recuperate()
    {
        Destroy(gameObject);
    }
    #endregion

    #region IDisplayable
    public void ActiveDescription()
    {
        canvasDisplay.SetActive(true);
    }

    public void DesactiveDescription()
    {
        canvasDisplay.SetActive(false);
    }
    #endregion
}