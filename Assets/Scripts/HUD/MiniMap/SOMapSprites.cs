using UnityEngine;

[CreateAssetMenu(fileName = "NewDataMapSprite", menuName = "Map Sprite", order = 1)]
public class SOMapSprites : ScriptableObject
{
    [Header("Sprites Room")]
    [SerializeField] private Sprite visitedRoom;
    [SerializeField] private Sprite actualRoom;
    [SerializeField] private Sprite nearestRoom;

    [Header("Sprites Type Room")]
    [SerializeField] private Sprite bossSkull;
    [SerializeField] private Sprite merchantCoin;

    public Texture2D VisitedRoom { get => visitedRoom.texture; }
    public Texture2D ActualRoom { get => actualRoom.texture; }
    public Texture2D NearestRoom { get => nearestRoom.texture; }
    public Texture2D BossSkull { get => bossSkull.texture; }
    public Texture2D MerchantCoin { get => merchantCoin.texture; }
}