using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    public float jumpForce = 5f;
    public float gravity = -9.81f;

    public float dashCooldown = 1f;

    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private float lastDashTime;

    [SerializeField] private WeaponSystem weapon;

    private bool isDashing;
    private Vector3 dashDirection;
    private float dashTimer;

    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashSpeed = 25f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        Jump();
        Move();
        Gravity();

        DashMovement();

        if (Input.GetMouseButtonDown(0))
        {
            weapon.Shoot();
        }

        Finisher();
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void Gravity()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)
            && Time.time > lastDashTime + dashCooldown
            && !isDashing)
        {
            bool success = RhythmManager.Instance.TryHit(BeatAction.Dash);

            dashDirection = transform.forward;
            isDashing = true;
            dashTimer = dashDuration;

            lastDashTime = Time.time;

            // TODO: perfect / good / miss feedback
        }
    }

    void DashMovement()
    {
        Dash();

        if (isDashing)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
    }

    void Finisher()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            bool success = RhythmManager.Instance.TryHit(BeatAction.Finisher);

            // logique Finisher
        }
    }
}