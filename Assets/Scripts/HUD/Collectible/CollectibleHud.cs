using TMPro;
using UnityEngine;

public class CollectibleHud : MonoBehaviour
{
    [Header("Text Value")]
    [SerializeField] private TextMeshProUGUI goldValue;
    [SerializeField] private TextMeshProUGUI bombValue;
    [SerializeField] private TextMeshProUGUI keyValue;

    private void Start()
    {
        LevelManager.Instance.onCollectibleChanged += OnCollectibleChanged;
    }

    private void OnCollectibleChanged(int gold,int bomb,int key)
    {
        goldValue.text = gold.ToString("D2");
        bombValue.text = bomb.ToString("D2");
        keyValue.text = key.ToString("D2");
    }
}