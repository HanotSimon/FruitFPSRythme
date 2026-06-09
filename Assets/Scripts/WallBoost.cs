using UnityEngine;

public class WallBoost : MonoBehaviour
{
    [SerializeField] private float launchPlayerForce = 20f;
    [SerializeField] private float launchOtherForce = 10f;
    [SerializeField] private float boostDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.LaunchHorizontal(-other.transform.forward, launchPlayerForce, boostDuration);
            }
        }
        else
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
                rb.AddForce(-other.transform.forward * launchOtherForce, ForceMode.Impulse);
            }
        }
    }
}