using System;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    public BeatmapData beatmap;

    private AudioSource audioSource;

    public double timeBeforeStart = 1.0;

    private double songStartDSPTime;

    public Action<BeatEvent> OnBeatReached;

    private int nextBeatIndex;

    public BeatEvent currentBeat;
    
    public bool beatActive;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void LoadLevel(LevelData level)
    {
        beatmap = level.beatmap;

        audioSource.clip = level.beatmap.musicClip;

        songStartDSPTime = AudioSettings.dspTime + timeBeforeStart;

        audioSource.PlayScheduled(songStartDSPTime);
    }

    private void Update()
    {
        double currentTime = GetSongTime();

        if (nextBeatIndex >= beatmap.beatEvents.Count)
            return;

        BeatEvent beat = beatmap.beatEvents[nextBeatIndex];

        if (currentTime >= beat.time)
        {
            currentBeat = beat;
            beatActive = true;

            OnBeatReached?.Invoke(beat);
            nextBeatIndex++;
        }

        if (beatActive && currentTime > currentBeat.time + beatmap.goodWindow)
        {
            beatActive = false;
            currentBeat = null;
        }
    }

    public double GetSongTime()
    {
        return AudioSettings.dspTime - songStartDSPTime + beatmap.musicOffset;
    }

    public bool TryHit(BeatAction action)
    {
        if (beatmap == null || beatmap.beatEvents == null)
            return false;

        if (nextBeatIndex >= beatmap.beatEvents.Count)
            return false;

        BeatEvent beat = beatmap.beatEvents[nextBeatIndex];

        if (beat.action != action)
            return false;

        double currentTime = GetSongTime();
        float diff = Mathf.Abs((float)(beat.time - currentTime));

        if (diff > beatmap.goodWindow)
        {
            Debug.Log("❌ MISS");
            return false;
        }

        if (diff <= beatmap.perfectWindow)
            Debug.Log("💥 PERFECT");
        else
            Debug.Log("👍 GOOD");

        nextBeatIndex++;
        return true;
    }
}