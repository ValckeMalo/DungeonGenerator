using UnityEngine;

[CreateAssetMenu(fileName = "NewPassiveItem", menuName = "Passive Item", order = 4)]
public class SOPassiveItem : ScriptableObject
{
    [Header("UI")]
    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get => sprite; }


    [Header("Statistic")]
    [SerializeField] private int lifeAddOn;
    public int LifeAddOn { get => lifeAddOn; }
}