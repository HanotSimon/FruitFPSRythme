// BeatmapRenderer.cs
using UnityEngine;
using System.Collections.Generic;

public class BeatmapRenderer : MonoBehaviour
{
    public GameObject beatPrefab;
    public RectTransform container;
    public RectTransform hitLine;
    public float pixelsPerSecond = 400f;

    [Tooltip("Combien de secondes avant le beat on spawne le visuel")]
    public float spawnWindow = 2f;

    private int nextSpawnIndex = 0;
    private List<GameObject> toDestroy = new List<GameObject>();

    private void Start()
    {
        BeatUI.hitBeats.Clear();
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
        CreateBeatUI(beat, -800f, index);
        CreateBeatUI(beat, +800f, index);
    }

    void CreateBeatUI(BeatEvent beat, float startX, int index)
    {
        GameObject obj = Instantiate(beatPrefab, container);
        BeatUI ui = obj.GetComponent<BeatUI>();
        RectTransform rt = obj.GetComponent<RectTransform>();

        ui.beatTime  = beat.time;
        ui.action    = beat.action;
        ui.beatIndex = index;
        ui.startX    = startX;

        rt.anchoredPosition = new Vector2(startX, 0);

        var img = obj.transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        img.color = beat.action switch
        {
            BeatAction.Shoot    => Color.red,
            BeatAction.Dash     => Color.cyan,
            BeatAction.Finisher => Color.yellow,
            _                   => Color.white
        };
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

            float direction = ui.startX < 0f ? 1f : -1f;
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
        toDestroy.Clear();

        foreach (Transform child in container)
        {
            BeatUI ui = child.GetComponent<BeatUI>();
            if (ui == null) continue;

            float timeToBeat = ui.beatTime - (float)songTime;

            if (Mathf.Abs(timeToBeat) < hitThreshold
                && !BeatUI.hitBeats.Contains(ui.beatIndex)
                && ui.TryMarkDestroyed())
            {
                BeatUI.hitBeats.Add(ui.beatIndex);
                toDestroy.Add(child.gameObject);
            }
        }

        foreach (var go in toDestroy)
            Destroy(go);
    }
}