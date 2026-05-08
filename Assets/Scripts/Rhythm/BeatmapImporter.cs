using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class BeatmapImporter
{
    public static List<BeatEvent> ImportLabels(string path)
    {
        List<BeatEvent> beats = new List<BeatEvent>();

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            string[] parts = line.Split('\t');

            if (parts.Length < 3)
                continue;

            if (!float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float time))
                continue;

            string label = parts[2].Trim();

            if (!System.Enum.TryParse(label, true, out BeatAction action))
                continue;

            beats.Add(new BeatEvent
            {
                time = time,
                action = action
            });
        }

        beats.Sort((a, b) => a.time.CompareTo(b.time));

        return beats;
    }
}