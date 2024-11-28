using UnityEngine;

public abstract class ActiveItem : MonoBehaviour
{
    [Header("Use")]
    [SerializeField] private int maxUse;
    [SerializeField] private int remainUse;

    public int MaxUse { get => maxUse; }
    public int RemainUse { get => remainUse; }

    [Header("UI")]
    [SerializeField] private Sprite sprite;

    public Sprite Sprite { get => sprite; }

    public virtual bool CanActiveEffect()
    {
        if (remainUse > 0)
        {
            remainUse--;
            ActiveEffect();
            return true;
        }

        return false;
    }

    protected abstract void ActiveEffect();
}