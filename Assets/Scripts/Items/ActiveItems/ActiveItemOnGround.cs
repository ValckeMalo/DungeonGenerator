using UnityEngine;
using UnityEngine.UI;

public class ActiveItemOnGround : MonoBehaviour, IRecuperable, IDisplayable
{
    [Header("Canvas")]
    [SerializeField] private GameObject canvasDisplay;
    [SerializeField] private Image iconActiveItem;

    [Header("Sprite In Scene")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Active Item")]
    [SerializeField] private GameObject activeItem;
    [SerializeField] private Transform containerActiveItem;
    [SerializeField] private EItem.ItemType type;

    public void InitializeItemOnGroud(GameObject containerActiveItem,ActiveItem activeItem)
    {
        containerActiveItem.transform.SetParent(this.containerActiveItem);
        containerActiveItem.transform.SetAsLastSibling();

        iconActiveItem.sprite = activeItem.Sprite;
        spriteRenderer.sprite = activeItem.Sprite;

        DesactiveDescription();
    }

    public void ChangeParentActiveItem(Transform newParent)
    {
        activeItem.transform.SetParent(newParent);
        activeItem.transform.SetAsLastSibling();

        activeItem = null;
    }

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