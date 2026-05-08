using System.IO;
using UnityEngine;

public class BeatmapAutoLoader : MonoBehaviour
{
    public BeatmapData beatmap;

    private void Awake()
    {
        LoadTxtIntoBeatmap();
    }

    void LoadTxtIntoBeatmap()
    {
        if (beatmap == null)
        {
            Debug.LogError("No Beatmap assigned");
            return;
        }

        string path = Application.dataPath + "/Beatmaps/" + beatmap.name + ".txt";

        if (!File.Exists(path))
        {
            Debug.LogError("TXT not found: " + path);
            return;
        }

        beatmap.beatEvents = BeatmapImporter.ImportLabels(path);

        Debug.Log("Beatmap loaded: " + beatmap.beatEvents.Count + " beats");
    }
}