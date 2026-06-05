using UnityEngine;
using System.Collections.Generic;

public class BeatUI : MonoBehaviour
{
    public float beatTime;
    public BeatAction action;
    public int beatIndex;

    [HideInInspector] public float startX;

    public static HashSet<int> hitBeats = new HashSet<int>();

    private bool isDestroyed = false;

    public bool TryMarkDestroyed()
    {
        if (isDestroyed) return false;
        isDestroyed = true;
        return true;
    }
}