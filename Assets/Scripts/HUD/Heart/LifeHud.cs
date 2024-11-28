using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeHud : MonoBehaviour
{
    [Header("Heart Hud")]
    [SerializeField] private GameObject prefabHeart;

    [Header("Sprites Heart")]
    [SerializeField] private SOHeart heartSprites;
    [SerializeField] private List<Image> heartImages;

    private void Start()
    {
        LevelManager.Instance.onLifeChanged += OnLifeChanged;
    }

    private void OnLifeChanged(List<Heart> newLife)
    {
        if (newLife.Count > heartImages.Count)
        {
            AddHeart(newLife.Count - heartImages.Count);
        }
        else if (newLife.Count < heartImages.Count)
        {
            RemoveHeart(heartImages.Count - newLife.Count);
        }

        UpdateHeartSprite(newLife);
    }

    private void AddHeart(int nbToAdd)
    {
        for (int i = 0; i < nbToAdd; i++)
        {
            GameObject newHeart = Instantiate(prefabHeart, transform);
            heartImages.Add(newHeart.GetComponent<Image>());
        }
    }

    private void RemoveHeart(int nbToRemove)
    {
        for (int i = 0; i < nbToRemove; i++)
        {
            heartImages.RemoveAt(0);
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    private void UpdateHeartSprite(List<Heart> newHeart)
    {
        for (int i = 0; i < newHeart.Count; i++)
        {
            heartImages[i].sprite = GetSpriteHeart(newHeart[i].State, newHeart[i].Type);
        }
    }

    private Sprite GetSpriteHeart(Heart.HeartState state, Heart.HeartType type)
    {
        if (state == Heart.HeartState.Empty)
        {
            return heartSprites.EmptyHeart;
        }

        switch (type)
        {
            case Heart.HeartType.Red:
                return GetHeartSpriteOnState(heartSprites.RedHeart, state);

            case Heart.HeartType.Black:
                return GetHeartSpriteOnState(heartSprites.BlackHeart, state);

            default:
                Debug.LogError("no type for the heart display");
                return GetHeartSpriteOnState(heartSprites.RedHeart, state);
        }
    }

    private Sprite GetHeartSpriteOnState(HeartSprite typeHeart, Heart.HeartState state)
    {
        switch (state)
        {
            case Heart.HeartState.Empty:
                return heartSprites.EmptyHeart;

            case Heart.HeartState.Half:
                return typeHeart.HalfHeart;

            case Heart.HeartState.Full:
                return typeHeart.FullHeart;

            default:
                Debug.LogError("no state for the heart display");
                return heartSprites.EmptyHeart;
        }
    }
}