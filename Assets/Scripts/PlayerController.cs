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
    [SerializeField] private LayerMask fruitLayer;

    private bool isDashing;
    private Vector3 dashDirection;
    private float dashTimer;

    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashSpeed = 25f;

    private bool isFinishing;
    private Vector3 finishDirection;
    private float finishTimer;

    private Vector3 boostVelocity;
    private float boostDuration;

    [SerializeField] private float finishSpeed = 40f;
    [SerializeField] private float finishDuration = 0.2f;
    [SerializeField] private float finishRange = 100f;

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

        Dash();
        DashMovement();
        FinisherMovement();

        ApplyBoost();

        if (Input.GetMouseButtonDown(0))
        {
            weapon.Shoot();
        }

        Finisher();
    }

    public void Launch(float force)
    {
        velocity.y = force;
    }

    public void LaunchHorizontal(Vector3 direction, float force, float duration)
    {
        boostVelocity = direction * force;
        boostDuration = duration;
    }

    void ApplyBoost()
    {
        if (boostVelocity == Vector3.zero) return;

        controller.Move(boostVelocity * Time.deltaTime);
        boostVelocity = Vector3.Lerp(boostVelocity, Vector3.zero, boostDuration * Time.deltaTime);

        if (boostVelocity.magnitude < 0.1f)
            boostVelocity = Vector3.zero;
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
            BeatResult beatResult = RhythmManager.Instance.TryHit(BeatAction.Dash);

            if (beatResult == BeatResult.Miss)
                return;

            dashDirection = transform.forward;
            isDashing = true;
            dashTimer = dashDuration;

            lastDashTime = Time.time;

            // TODO: feedback (perfect/good)
        }
    }

    void DashMovement()
    {
        if (!isDashing)
            return;

        controller.Move(dashDirection * dashSpeed * Time.deltaTime);

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    void Finisher()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isFinishing)
        {
            BeatResult beatResult = RhythmManager.Instance.TryHit(BeatAction.Finisher);

            if (beatResult == BeatResult.Miss)
                return;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, finishRange, fruitLayer))
            {
                finishDirection = (hit.transform.position - transform.position).normalized;

                isFinishing = true;
                finishTimer = finishDuration;

                Destroy(hit.collider.gameObject);
            }
        }
    }

    void FinisherMovement()
    {
        if (!isFinishing)
            return;

        controller.Move(finishDirection * finishSpeed * Time.deltaTime);

        finishTimer -= Time.deltaTime;

        if (finishTimer <= 0f)
        {
            isFinishing = false;
        }
    }
}