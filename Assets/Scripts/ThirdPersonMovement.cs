using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    private float fallStartY;
    private bool isFalling = false;
    [Header("Jump Tuning")]
public float fallMultiplier = 2.5f; // makes falling faster
public float lowJumpMultiplier = 2f; // optional: makes short taps feel responsive

    [Header("Fall Damage")]
    public float minFallDamageHeight = 3f;
    public float maxFallDamageHeight = 10f;
    public int maxFallDamage = 50;
    private PlayerHealth playerHealth;
    [Header("Movement")]
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float jumpForce = 7f;
    public float turnSmoothTime = 0.1f;

    [Header("Gravity & Ground Check")]
    public float gravity = -9.81f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private float turnSmoothVelocity;
    private float verticalVelocity = 0f;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private float originalSpeed;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        originalSpeed = speed;
        playerHealth = GetComponent<PlayerHealth>();

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = gc.transform;
        }
    }

    void Update()
    {
        GroundCheck();
        HandleInput();
        ApplyGravity();
        MovePlayer();
        UpdateAnimations();
    }

    void GroundCheck()
{
    bool wasGrounded = isGrounded;
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    if (!wasGrounded && isGrounded)
    {
        // Landed
        float fallDistance = fallStartY - transform.position.y;
        if (fallDistance > minFallDamageHeight)
        {
            float t = Mathf.InverseLerp(minFallDamageHeight, maxFallDamageHeight, fallDistance);
            int damage = Mathf.RoundToInt(Mathf.Lerp(0, maxFallDamage, t));
            playerHealth.TakeDamage(damage);
        }

        isFalling = false;
    }

    if (wasGrounded && !isGrounded)
    {
        // Started falling
        fallStartY = transform.position.y;
        isFalling = true;
    }

    if (isGrounded && verticalVelocity < 0)
    {
        verticalVelocity = -2f; // more natural landing
    }
}


    void HandleInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }
    }

    void Jump()
    {
        verticalVelocity = jumpForce;
        animator.SetTrigger("ToggleJump");

    }

    void ApplyGravity()
{
    if (isGrounded && verticalVelocity < 0)
    {
        verticalVelocity = -2f; // Keeps player grounded
    }

    if (verticalVelocity < 0)
    {
        // Falling
        verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
    }
    else if (!Input.GetButton("Jump"))
    {
        // Let go of jump early
        verticalVelocity += gravity * lowJumpMultiplier * Time.deltaTime;
    }
    else
    {
        verticalVelocity += gravity * Time.deltaTime;
    }
}


    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        Vector3 velocity = new Vector3(0, verticalVelocity, 0);
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

        animator.SetBool("IsRunning", inputDir.magnitude >= 0.1f);
        animator.SetBool("Iscrouching", isCrouching);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsFalling", !isGrounded && verticalVelocity < 0);

        // Reset jump trigger once airborne
        
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        controller.height = isCrouching ? 1.0f : 2.0f;
        controller.center = new Vector3(0, isCrouching ? 0.5f : 1.0f, 0);
        speed = isCrouching ? (originalSpeed * 0.5f) : originalSpeed;
        animator.SetBool("Iscrouching", isCrouching);
    }
}
