using UnityEngine;

public class PricePanelToggle : MonoBehaviour
{
    PlayerControls controls;
    UIManager ui;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Price.performed += _ => TogglePrice();

        ui = FindObjectOfType<UIManager>(true);
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void TogglePrice()
    {
        if (ui == null) return;

        var panel = ui.GetComponentInChildren<PricePanel>(true);
        if (panel == null) return;

        bool on = !panel.gameObject.activeSelf;
        if (on) ui.ShowPricePanel();
        else panel.gameObject.SetActive(false);
    }
}
