using System.Collections;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    #region Singleton
    private static FadeManager instance;
    public static FadeManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(gameObject);
    }
    #endregion

    public void StartFadeRoom()
    {
        StartCoroutine(FadeRoom());
    }

    private IEnumerator FadeRoom()
    {
        yield return new WaitForSeconds(0.2f);
        LevelManager.Instance.IsOnHalfFade = true;
        yield return new WaitForSeconds(0.2f);
        LevelManager.Instance.FadeFinish = true;
    }
}