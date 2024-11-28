using UnityEngine;
using UnityEngine.UI;

public class PassiveItemHud : MonoBehaviour
{
    #region Singleton
    private static PassiveItemHud instance;
    public static PassiveItemHud Instance { get => instance; }

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

    [Header("Passive Item")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform containerPassiveItem;

    private void Start()
    {
        LevelManager.Instance.onPassiveItemChanged += AddPassiveItemInventory;
    }

    public void ShowPassiveItem()
    {
        containerPassiveItem.gameObject.SetActive(true);
    }

    public void HidePassiveitem()
    {
        containerPassiveItem.gameObject.SetActive(false);
    }

    private void AddPassiveItemInventory(SOPassiveItem newPassiveItem)
    {
        InitializePrefabPassiveItem(newPassiveItem, Instantiate(prefab, containerPassiveItem));
    }

    private void InitializePrefabPassiveItem(SOPassiveItem itemToConvert, GameObject newPrefab)
    {
        newPrefab.GetComponent<Image>().sprite = itemToConvert.Sprite;
    }
}