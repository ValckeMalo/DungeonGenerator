
public interface IHealable
{
    public bool HealHeart(int nbHeal,Heart.HeartType type);

    public void AddHeart(Heart.HeartType type);

    public void RemoveHeart(Heart.HeartType type);
}