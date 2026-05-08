using UnityEngine;
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

    public void Shoot()
    {
        audioSource.PlayOneShot(failSound);
        
        bool rhythmSuccess = RhythmManager.Instance.TryHit(BeatAction.Shoot);

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

            if (rhythmSuccess)
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
}