using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BeatmapRenderer : MonoBehaviour
{
    public static BeatmapRenderer Instance;

    public GameObject beatShootPrefab;
    public GameObject beatDashPrefab;
    public GameObject beatFinisherPrefab;

    public RectTransform container;
    public RectTransform hitLine;
    public float pixelsPerSecond = 400f;

    public float spawnWindow = 2f;

    public Image goodWindowIndicator;
    public Image perfectWindowIndicator;

    private int nextSpawnIndex = 0;
    private List<GameObject> toDestroy = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        var beatmap = RhythmManager.Instance.beatmap;
        double songTime = RhythmManager.Instance.GetSongTime();

        if (beatmap == null) return;

        SpawnUpcoming(beatmap, songTime);
        MoveAndCleanBeats(songTime);
    }

    void SpawnUpcoming(BeatmapData beatmap, double songTime)
    {
        while (nextSpawnIndex < beatmap.beatEvents.Count)
        {
            BeatEvent beat = beatmap.beatEvents[nextSpawnIndex];

            if (beat.time - songTime > spawnWindow)
                break;

            if (beat.time - songTime < -1f)
            {
                nextSpawnIndex++;
                continue;
            }

            CreateBeatPair(beat, nextSpawnIndex);
            nextSpawnIndex++;
        }
    }

    void CreateBeatPair(BeatEvent beat, int index)
    {
        CreateBeatUI(beat, true, index);
        CreateBeatUI(beat, false, index);
    }

    void CreateBeatUI(BeatEvent beat, bool fromLeft, int index)
    {
        GameObject obj = beat.action switch
        {
            BeatAction.Shoot => Instantiate(beatShootPrefab, container),
            BeatAction.Dash => Instantiate(beatDashPrefab, container),
            BeatAction.Finisher => Instantiate(beatFinisherPrefab, container),
            _ => Instantiate(beatShootPrefab, container)
        };

        BeatUI ui = obj.GetComponent<BeatUI>();
        RectTransform rt = obj.GetComponent<RectTransform>();

        ui.beatTime = beat.time;
        ui.action = beat.action;
        ui.beatIndex = index;
        ui.fromLeft = fromLeft;

        rt.anchoredPosition = new Vector2(
            fromLeft ? -spawnWindow * pixelsPerSecond
                     : spawnWindow * pixelsPerSecond,
            0
        );

        rt.localScale = new Vector3(
            fromLeft ? 1 : -1,
            1,
            1
        );
    }

    void MoveAndCleanBeats(double songTime)
    {
        toDestroy.Clear();

        foreach (Transform child in container)
        {
            BeatUI ui = child.GetComponent<BeatUI>();
            if (ui == null) continue;

            RectTransform rt = child.GetComponent<RectTransform>();
            float timeToBeat = ui.beatTime - (float)songTime;

            float direction = ui.fromLeft ? 1f : -1f;
            rt.anchoredPosition = new Vector2(direction * timeToBeat * pixelsPerSecond, 0);

            bool atCenter = Mathf.Abs(timeToBeat) < 0.05f;
            bool tooLate  = timeToBeat < -1f;

            if ((atCenter || tooLate) && ui.TryMarkDestroyed())
                toDestroy.Add(child.gameObject);
        }

        foreach (var go in toDestroy)
            Destroy(go);
    }

    public void OnPlayerHit(float hitThreshold)
    {
        double songTime = RhythmManager.Instance.GetSongTime();
        int targetIndex = RhythmManager.Instance.nextBeatIndex;
        toDestroy.Clear();

        foreach (Transform child in container)
        {
            BeatUI ui = child.GetComponent<BeatUI>();
            if (ui == null) continue;
            if (ui.beatIndex == targetIndex && ui.TryMarkDestroyed())
                toDestroy.Add(child.gameObject);
        }

        foreach (var go in toDestroy)
            Destroy(go);
    }

    public void UpdateWindowIndicators()
    {
        var beatmap = RhythmManager.Instance.beatmap;
        if (beatmap == null) return;

        float goodWidth = (float)beatmap.goodWindow * pixelsPerSecond * 2f;
        float perfectWidth = (float)beatmap.perfectWindow * pixelsPerSecond * 2f;

        if (goodWindowIndicator)
            goodWindowIndicator.rectTransform.sizeDelta =
                new Vector2(goodWidth, goodWindowIndicator.rectTransform.sizeDelta.y);

        if (perfectWindowIndicator)
            perfectWindowIndicator.rectTransform.sizeDelta =
                new Vector2(perfectWidth, perfectWindowIndicator.rectTransform.sizeDelta.y);
    }
}