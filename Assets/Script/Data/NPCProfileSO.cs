using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Profile")]
public class NPCProfileSO : ScriptableObject
{
    public string displayName;
    public Personality personality;

    [Range(0, 100)] public int moodStart;
    [Range(0, 100)] public int moodTarget;

    [TextArea(2, 4)] public string[] greetLines;
    [TextArea(2, 4)] public string[] successLines;
    [TextArea(2, 4)] public string[] failLines;
}
