using UnityEngine;

public class GlobalUIBootstrap : MonoBehaviour
{
    public static GlobalUIBootstrap I { get; set; }
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }
}