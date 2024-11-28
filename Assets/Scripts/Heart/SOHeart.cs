using UnityEngine;

[CreateAssetMenu(fileName = "NewHeartSprites", menuName = "Heart", order = 3)]
public class SOHeart : ScriptableObject
{
    [Header("Empty Heart")]
    [SerializeField] private Sprite emptyHeart;

    public Sprite EmptyHeart { get => emptyHeart; }

    [Header("Heart Sprite")]
    [SerializeField] private HeartSprite redHeart;
    [SerializeField] private HeartSprite blackHeart;

    public HeartSprite RedHeart { get => redHeart; }
    public HeartSprite BlackHeart { get => blackHeart; }
}

[System.Serializable]
public class HeartSprite
{
    [Header("Sprite Heart")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite halfHeart;

    public Sprite FullHeart { get => fullHeart; }
    public Sprite HalfHeart { get => halfHeart; }
}