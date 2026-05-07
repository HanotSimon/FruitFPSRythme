using UnityEngine;

public class FruitMovement : MonoBehaviour
{
    [Header("Perlin Movement")]
    public float amplitude = 0.5f;
    public float frequency = 1f;

    private Vector3 startPos;
    private float offsetX;
    private float offsetZ;

    [Header("Rotation (optional)")]
    public bool enableRotation = false;
    public float rotationSpeed = 20f;

    void OnEnable()
    {
        startPos = transform.position;

        offsetX = Random.Range(0f, 1000f);
        offsetZ = Random.Range(0f, 1000f);
    }

    void Update()
    {
        MovePerlin();
        HandleRotation();
    }

    void MovePerlin()
    {
        float x = Mathf.PerlinNoise(Time.time * frequency + offsetX, 0) - 0.5f;
        float z = Mathf.PerlinNoise(0, Time.time * frequency + offsetZ) - 0.5f;

        Vector3 offset = new Vector3(x, 0, z) * amplitude;

        transform.position = startPos + offset;
    }

    void HandleRotation()
    {
        if (!enableRotation) return;

        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}