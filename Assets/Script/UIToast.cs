using UnityEngine;
using TMPro;

public static class UIToast
{
    static TextMeshProUGUI txt;

    public static void Show(string msg, float time = 2f)
    {
        if (txt == null)
        {
            var go = GameObject.Find("ToastText");
            if (go == null) { Debug.LogWarning("ToastText UI not found"); return; }
            txt = go.GetComponent<TextMeshProUGUI>();
        }

        txt.text = msg;
        txt.canvasRenderer.SetAlpha(1);
        txt.CrossFadeAlpha(0, time, false);
    }
}
