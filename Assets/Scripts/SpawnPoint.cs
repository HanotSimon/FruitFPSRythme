using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] fruitPrefabs;

    public float spawnInterval = 2f;

    public bool randomRotation = true;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), Random.Range(0f, 1f), spawnInterval);
    }

    void Spawn()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
            return;

        GameObject fruit = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

        Quaternion rotation = transform.rotation;

        if (randomRotation)
            rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        Instantiate(fruit, transform.position, rotation);
    }
}