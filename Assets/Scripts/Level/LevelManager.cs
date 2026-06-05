using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Levels (TEST)")]
    public LevelData level1;
    public LevelData level2;

    private static LevelData selectedLevel;

    void Start()
    {
        // Premier lancement → défaut
        if (selectedLevel == null)
            selectedLevel = level1;

        LoadLevel(selectedLevel);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ReloadWithLevel(level1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ReloadWithLevel(level2);
        }
    }

    void ReloadWithLevel(LevelData level)
    {
        selectedLevel = level;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel(LevelData level)
    {
        if (level == null)
        {
            Debug.LogError("No level assigned!");
            return;
        }

        // Load arena
        Instantiate(
            level.arenaPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        // Load rhythm system
        RhythmManager.Instance.LoadLevel(level);

        Debug.Log("Loaded level: " + level.levelName);
    }
}