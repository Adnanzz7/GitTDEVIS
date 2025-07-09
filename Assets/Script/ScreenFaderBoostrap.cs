using UnityEngine;

public class ScreenFaderBootstrap : MonoBehaviour
{
    [SerializeField] ScreenFader faderPrefab;

    void Awake()
    {
        if (FindObjectOfType<ScreenFader>() == null)
            Instantiate(faderPrefab);
    }
}
