using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float range = 100f;
    [SerializeField] private LayerMask fruitLayer;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private CameraShake camShake;

    [SerializeField] private AudioClip killSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private Image crosshair;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color targetColor = Color.red;

    void Update()
    {
        UpdateCrosshair();
    }

    public void Shoot()
    {
        audioSource.PlayOneShot(failSound);

        BeatResult beatResult = RhythmManager.Instance.TryHit(BeatAction.Shoot);

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range, fruitLayer))
        {
            GameObject effect = Instantiate(
                hitEffectPrefab,
                hit.point,
                Quaternion.LookRotation(hit.normal)
            );

            Destroy(effect, 1f);

            if (beatResult != BeatResult.Miss)
            {
                audioSource.PlayOneShot(killSound);
            }

            Destroy(hit.collider.gameObject);
        }
        else
        {
            StartCoroutine(camShake.Shake(0.1f, 0.05f));
        }
    }

    void UpdateCrosshair()
    {
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range, fruitLayer))
        {
            crosshair.color = targetColor;
        }
        else
        {
            crosshair.color = normalColor;
        }
    }
}