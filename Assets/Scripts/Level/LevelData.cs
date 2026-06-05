using UnityEngine;

[CreateAssetMenu(menuName = "Core/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("General")]
    public string levelName;

    [Header("Beatmap")]
    public BeatmapData beatmap;

    [Header("Environment")]
    public GameObject arenaPrefab;
}