using UnityEngine;
using UnityEngine.UI;

public class ActiveItemHud : MonoBehaviour
{
    [Header("Active Item")]
    [SerializeField] private Image imageActiveItem;
    [SerializeField] private GameObject prefabRod;
    [SerializeField] private Transform containerRod;
    [SerializeField] private Slider sliderUseItem;
    [SerializeField] private GameObject cotnainerActiveItem;

    private void Start()
    {
        LevelManager.Instance.onActiveItemChanged += OnActiveItemChanged;
    }

    private void OnActiveItemChanged(ActiveItem activeItem)
    {
        if (activeItem != null)
        {
            if (!cotnainerActiveItem.activeSelf)
            {
                cotnainerActiveItem.SetActive(true);
            }

            imageActiveItem.sprite = activeItem.Sprite;
            UpdateUseRod(activeItem.RemainUse, activeItem.MaxUse);
        }
        else
        {
            cotnainerActiveItem.SetActive(false);
        }
    }

    private void UpdateUseRod(int remainUse, int maxUse)
    {
        sliderUseItem.value = remainUse;
        sliderUseItem.maxValue = maxUse;

        if (containerRod.childCount != maxUse - 1)
        {
            UpdateRodSlider(maxUse - 1);
        }
    }

    private void UpdateRodSlider(int nbRod)
    {
        int nbCurrentChild = containerRod.childCount;
        for (int i = 0; i < nbCurrentChild; i++)
        {
            Destroy(containerRod.GetChild(0).gameObject);
        }

        for (int i = 0; i < nbRod; i++)
        {
            Instantiate(prefabRod, containerRod);
        }
    }
}