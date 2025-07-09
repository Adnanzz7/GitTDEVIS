using UnityEngine;
using TMPro;

public class TradeTimer : MonoBehaviour
{
    [SerializeField] float sessionSeconds = 120f;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] RecapPanel recapPanel;
    [SerializeField] BuyerWaveManager wave;

    float _remain;
    bool _running;

    void Start()
    {
        SalesStats.Reset();
        _remain = sessionSeconds;
        _running = true;
    }

    void Update()
    {
        if (!_running) return;
        _remain -= Time.deltaTime;
        if (_remain < 0f)
        {
            _remain = 0f;
            _running = false;
            wave.StopWaves();
            recapPanel.ShowRecap();
        }
        int m = Mathf.FloorToInt(_remain / 60f);
        int s = Mathf.FloorToInt(_remain % 60f);
        timerText.text = $"{m:00}:{s:00}";
    }
}
