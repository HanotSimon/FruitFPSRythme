using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float launchPlayerForce = 20f;
    [SerializeField] private float launchOtherForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.Launch(launchPlayerForce);
            }
        }
        else
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = new Vector3(
                    rb.linearVelocity.x,
                    0f,
                    rb.linearVelocity.z);

                rb.AddForce(Vector3.up * launchOtherForce, ForceMode.Impulse);
            }
        }
    }
}