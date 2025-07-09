using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class LiveSortDebug : MonoBehaviour
{
    SpriteRenderer sr;
    void Awake() { sr = GetComponent<SpriteRenderer>(); }
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "order: " + sr.sortingOrder);
    }
}
