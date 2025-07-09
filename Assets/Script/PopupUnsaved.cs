using UnityEngine;
using UnityEngine.UI;

public class PopupUnsaved : MonoBehaviour
{
    static PopupUnsaved inst;

    [SerializeField] Text label;
    [SerializeField] Button btnYes;
    [SerializeField] Button btnNo;

    void Awake() { inst = this; gameObject.SetActive(false); }

    public static void Show(string msg, System.Action onQuit)
    {
        inst.label.text = msg;
        inst.btnYes.onClick.RemoveAllListeners();
        inst.btnYes.onClick.AddListener(() => { onQuit(); inst.Hide(); });
        inst.btnNo.onClick.RemoveAllListeners();
        inst.btnNo.onClick.AddListener(inst.Hide);
        inst.gameObject.SetActive(true);
    }

    void Hide() => gameObject.SetActive(false);
}
