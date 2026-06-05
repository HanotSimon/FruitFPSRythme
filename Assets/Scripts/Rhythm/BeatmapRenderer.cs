using UnityEngine;
using System.Collections.Generic;

public class BeatmapRenderer : MonoBehaviour
{
    public BeatmapData beatmap;
    public GameObject beatPrefab;
    public RectTransform container;
    public RectTransform hitLine;
    public float pixelsPerSecond = 400f;

    private List<GameObject> toDestroy = new List<GameObject>();

    private void Start()
    {
        BeatUI.hitBeats.Clear();
        SpawnBeats();
    }

    void SpawnBeats()
    {
        for (int i = 0; i < beatmap.beatEvents.Count; i++)
        {
            CreateBeatUI(beatmap.beatEvents[i], -800f, i);
            CreateBeatUI(beatmap.beatEvents[i], +800f, i);
        }
    }

    void CreateBeatUI(BeatEvent beat, float startX, int index)
    {
        GameObject obj = Instantiate(beatPrefab, container);
        BeatUI ui = obj.GetComponent<BeatUI>();
        RectTransform rt = obj.GetComponent<RectTransform>();

        ui.beatTime = beat.time;
        ui.action = beat.action;
        ui.beatIndex = index;
        ui.startX = startX;
        rt.anchoredPosition = new Vector2(startX, 0);

        var img = obj.GetComponent<UnityEngine.UI.Image>();
        img.color = beat.action switch
        {
            BeatAction.Shoot => Color.red,
            BeatAction.Dash => Color.cyan,
            BeatAction.Finisher => Color.yellow,
            _ => Color.white
        };
    }

    private void Update()
    {
        double songTime = RhythmManager.Instance.GetSongTime();
        toDestroy.Clear();

        foreach (Transform child in container)
        {
            BeatUI ui = child.GetComponent<BeatUI>();
            if (ui == null) continue;

            RectTransform rt = child.GetComponent<RectTransform>();
            float timeToBeat = ui.beatTime - (float)songTime;

            float direction = ui.startX < 0f ? 1f : -1f;
            float x = direction * timeToBeat * pixelsPerSecond;
            rt.anchoredPosition = new Vector2(x, 0);

            bool atCenter = Mathf.Abs(timeToBeat) < 0.05f;
            bool tooLate = timeToBeat < -1f;

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
                // scorer ici ou envoyer un event
            }
        }

        foreach (var go in toDestroy)
            Destroy(go);
    }
}