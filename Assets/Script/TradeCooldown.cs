using UnityEngine;

public static class TradeCooldown
{
    static float _readyAt;

    public static void SetCooldown(float seconds) => _readyAt = Time.realtimeSinceStartup + seconds;
    public static bool OnCooldown => Time.realtimeSinceStartup < _readyAt;
}
