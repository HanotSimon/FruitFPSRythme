using UnityEngine;

public class BeatmapRenderer : MonoBehaviour
{
    public BeatmapData beatmap;

    public GameObject beatPrefab;
    public RectTransform container;

    public RectTransform hitLine;

    public float pixelsPerSecond = 400f;

    private float centerX;

    private void Start()
    {
        centerX = hitLine.anchoredPosition.x;
        SpawnBeats();
    }

    void SpawnBeats()
    {
        foreach (var beat in beatmap.beatEvents)
        {
            GameObject obj = Instantiate(beatPrefab, container);

            BeatUI ui = obj.GetComponent<BeatUI>();
            ui.beatTime = beat.time;
            ui.action = beat.action;

            RectTransform rt = obj.GetComponent<RectTransform>();

            rt.anchoredPosition = new Vector2(
                -800,
                0
            );

            var img = obj.GetComponent<UnityEngine.UI.Image>();

            if (beat.action == BeatAction.Shoot)
                img.color = Color.red;

            if (beat.action == BeatAction.Dash)
                img.color = Color.cyan;

            if (beat.action == BeatAction.Finisher)
                img.color = Color.yellow;
        }
    }

    private void Update()
    {
        double songTime = RhythmManager.Instance.GetSongTime();

        foreach (Transform child in container)
        {
            BeatUI ui = child.GetComponent<BeatUI>();
            RectTransform rt = child.GetComponent<RectTransform>();

            float timeToBeat = ui.beatTime - (float)songTime;

            float x = timeToBeat * pixelsPerSecond;

            rt.anchoredPosition = new Vector2(
                x,
                0
            );

            if (timeToBeat < -1f)
            {
                Destroy(child.gameObject);
            }
        }
    }
}