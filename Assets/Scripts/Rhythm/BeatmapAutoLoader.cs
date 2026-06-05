using System.IO;
using UnityEngine;

public class BeatmapAutoLoader : MonoBehaviour
{
    private void Awake()
    {
        LoadTxtIntoBeatmap();
    }

    void LoadTxtIntoBeatmap()
    {
        var beatmap = RhythmManager.Instance.beatmap;

        if (beatmap == null)
        {
            Debug.LogError("No Beatmap assigned");
            return;
        }

        string path = Application.dataPath + "/ScriptableObjects/Beatmaps/" + beatmap.name + ".txt";

        if (!File.Exists(path))
        {
            Debug.LogError("TXT not found: " + path);
            return;
        }

        beatmap.beatEvents = BeatmapImporter.ImportLabels(path);

        Debug.Log("Beatmap loaded: " + beatmap.beatEvents.Count + " beats");
    }
}