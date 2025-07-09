using UnityEngine;
using TMPro;

public class UIDayLabel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dayLabel;

    void Start() => UpdateDay();
    void OnEnable() => DayManager.I.OnMorning += UpdateDay;
    void OnDisable() => DayManager.I.OnMorning -= UpdateDay;

    void UpdateDay() => dayLabel.text = $"Day {DayManager.I.CurrentDay}";
}
