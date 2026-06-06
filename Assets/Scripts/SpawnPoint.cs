using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject[] fruitPrefabs;

    public float spawnInterval = 2f;

    public bool randomRotation = true;

    public float spawnRadius = 2f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), Random.Range(0f, 1f), spawnInterval);
    }

    void Spawn()
    {
        if (fruitPrefabs == null || fruitPrefabs.Length == 0)
            return;

        GameObject fruit = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPos = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        Quaternion rotation = transform.rotation;

        if (randomRotation)
            rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

        Instantiate(fruit, spawnPos, rotation, transform);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        int segments = 40;
        float angleStep = 360f / segments;

        Vector3 previousPoint = transform.position + new Vector3(spawnRadius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;

            Vector3 nextPoint = transform.position + new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0f,
                Mathf.Sin(angle) * spawnRadius
            );

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}