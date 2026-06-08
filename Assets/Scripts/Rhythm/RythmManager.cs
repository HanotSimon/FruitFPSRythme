using System;
using System.IO;
using UnityEngine;
using System.Collections;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    public BeatmapData beatmap;

    private AudioSource audioSource;

    public double timeBeforeStart = 1.0;

    private double songStartDSPTime;

    public Action<BeatEvent> OnBeatReached;

    public int nextBeatIndex;

    public BeatEvent currentBeat;

    public bool beatActive;

    private double gameplayStartTime;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void LoadLevel(LevelData level)
    {
        beatmap = level.beatmap;

        LoadTxtIntoBeatmap();

        audioSource.clip = level.beatmap.musicClip;

        songStartDSPTime = AudioSettings.dspTime + timeBeforeStart;

        audioSource.PlayScheduled(songStartDSPTime);

        StartCoroutine(StartAudioWithOffset());

        BeatmapRenderer.Instance.UpdateWindowIndicators();
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

    private IEnumerator StartAudioWithOffset()
    {
        yield return new WaitUntil(() => AudioSettings.dspTime >= songStartDSPTime);

        audioSource.time = (float)beatmap.musicOffset;
    }

    public BeatResult TryHit(BeatAction action)
    {
        if (beatmap == null || beatmap.beatEvents == null)
            return BeatResult.Miss;

        if (nextBeatIndex >= beatmap.beatEvents.Count)
            return BeatResult.Miss;

        BeatEvent beat = beatmap.beatEvents[nextBeatIndex];

        if (beat.action != action)
            return BeatResult.Miss;

        double currentTime = GetSongTime();
        float diff = Mathf.Abs((float)(beat.time - currentTime));

        if (diff > beatmap.goodWindow)
        {
            UIFeedback.Instance.ShowFeedback(BeatResult.Miss);
            return BeatResult.Miss;
        }

        if (diff <= beatmap.perfectWindow)
        {
            UIFeedback.Instance.ShowFeedback(BeatResult.Perfect);
            BeatmapRenderer.Instance.OnPlayerHit((float)beatmap.goodWindow);
            ScoreManager.Instance.AddScore(100);
            nextBeatIndex++;
            return BeatResult.Perfect;
        }

        UIFeedback.Instance.ShowFeedback(BeatResult.Good);
        BeatmapRenderer.Instance.OnPlayerHit((float)beatmap.goodWindow);
        ScoreManager.Instance.AddScore(50);
        nextBeatIndex++;
        return BeatResult.Good;
    }

    void LoadTxtIntoBeatmap()
    {
        if (beatmap == null)
        {
            Debug.LogError("No Beatmap assigned");
            return;
        }

        Debug.Log("Loading beatmap: " + beatmap.name);

        string path = Application.dataPath +
            "/ScriptableObjects/Beatmaps/" +
            beatmap.name + ".txt";

        if (!File.Exists(path))
        {
            Debug.LogError("TXT not found: " + path);
            return;
        }

        beatmap.beatEvents = BeatmapImporter.ImportLabels(path);

        Debug.Log("Beatmap loaded: " + beatmap.beatEvents.Count + " beats");
    }
}

public enum BeatResult
{
    Miss,
    Good,
    Perfect
}