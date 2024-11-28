
public interface IRecuperable
{
     public void Recuperate();

    public EItem.ItemType GetTypeObject();
}

public class EItem
{
    public enum ItemType
    {
        Gold,
        Bomb,
        Key,
        ActiveItem,
        PassiveItem,
    }
}