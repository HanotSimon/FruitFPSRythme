using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rhythm/Beatmap")]
public class BeatmapData : ScriptableObject
{
    public AudioClip musicClip;

    [Header("Timing")]
    public float bpm = 120f;
    public float musicOffset = 0f;

    [Header("Windows")]
    public float perfectWindow = 0.08f;
    public float goodWindow = 0.15f;

    public List<BeatEvent> beatEvents;
}

public enum BeatAction
{
    Shoot,
    Dash,
    Finisher
}

[System.Serializable]
public class BeatEvent
{
    public float time;
    public BeatAction action;

    [HideInInspector]
    public bool hit;
}