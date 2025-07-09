using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] float fadeDuration = 0.5f;

    static ScreenFader instance;
    CanvasGroup cg;
    bool isFading;

    void Awake()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        cg = GetComponentInChildren<CanvasGroup>(true);
        cg.alpha = 0;
        cg.gameObject.SetActive(false);
    }

    public static void FadeToScene(string scene) => instance.StartCoroutine(instance.FadeRoutine(scene));
    public static void FadeToBlack(float t) => instance.StartCoroutine(instance.Fade(1, t));
    public static void FadeFromBlack(float t) => instance.StartCoroutine(instance.Fade(0, t));

    IEnumerator FadeRoutine(string scene)
    {
        if (isFading) yield break;
        isFading = true;

        cg.gameObject.SetActive(true);
        yield return Fade(1, fadeDuration);

        yield return SceneManager.LoadSceneAsync(scene);

        yield return Fade(0, fadeDuration);
        cg.gameObject.SetActive(false);
        isFading = false;
    }

    IEnumerator Fade(float target, float dur)
    {
        float start = cg.alpha;
        cg.blocksRaycasts = true;
        for (float t = 0; t < dur; t += Time.unscaledDeltaTime)
        {
            cg.alpha = Mathf.Lerp(start, target, t / dur);
            yield return null;
        }
        cg.alpha = target;
        cg.blocksRaycasts = target > 0.5f;
    }
}
