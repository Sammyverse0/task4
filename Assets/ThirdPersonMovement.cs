using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public float jumpForce = 10f;
    public float turnSmoothTime = 0.1f;

    [Header("Gravity & Ground Check")]
    public float gravity = -9.81f; // Fixed: Standard gravity value
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Fall Damage Settings")]
    public float fallDamageThreshold = -2f; // Trigger fall damage at this Y velocity
    public int maxFallDamage = 100;

    // Private variables
    private float turnSmoothVelocity;
    private bool isCrouching = false;
    private float verticalVelocity = 0f;
    private float lastYVelocity = 0f;
    private bool isGrounded = false;
    private float originalSpeed;

    // Component references
    private PlayerHealth playerHealth;
    private Animator animator;

    void Start()
    {
        // Get component references
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        
        // Store original speed for crouch toggle
        originalSpeed = speed;

        // Create ground check object if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = groundCheckObj.transform;
        }

        // Initialize animation states
        if (animator != null)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("Iscrouching", false);
        }
    }

    void Update()
    {
        // Check if player is dead
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            // Player is dead, skip movement and rotation
            if (animator != null)
            {
                animator.SetBool("IsRunning", false);
                animator.SetBool("Iscrouching", false);
            }
            return;
        }

        // Ground check - FIXED: Check before handling input
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Handle input
        HandleInput();

        // Save last vertical velocity for fall damage calculation
        lastYVelocity = verticalVelocity;

        // Handle gravity and fall damage
        HandleGravityAndFallDamage();

        // Handle movement
        HandleMovement();

        // Update animations
        UpdateAnimations();

        // Debug information
        Debug.Log($"IsGrounded: {isGrounded}, VerticalVelocity: {verticalVelocity:F2}");
    }

    void HandleInput()
    {
        // Jump input - FIXED: Added more input options and debug
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Jump input detected! IsGrounded: {isGrounded}");
            TriggerJump();
        }

        // Crouch input (C key or Left Control)
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }
    }

    void HandleGravityAndFallDamage()
    {
        if (isGrounded)
        {
            // Check for fall damage when landing
            if (lastYVelocity < fallDamageThreshold)
            {
                int damage = CalculateFallDamage(lastYVelocity);
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log($"Fall Damage Taken: {damage}");
                }
            }

            // FIXED: Better ground velocity handling
            if (verticalVelocity < 0)
            {
                verticalVelocity = -0.5f; // Small negative value to ensure we stay grounded
            }
        }
        else
        {
            // Apply gravity when in air
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    void HandleMovement()
    {
        // Get movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Handle horizontal movement
        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera direction
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            
            // Smooth rotation
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Move in the calculated direction
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // Apply vertical movement (gravity/jumping)
        Vector3 verticalMovement = Vector3.up * verticalVelocity;
        controller.Move(verticalMovement * Time.deltaTime);
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Update running animation based on input
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            
            animator.SetBool("IsRunning", direction.magnitude >= 0.1f);
            animator.SetBool("Iscrouching", isCrouching);
        }
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;
        
        // Adjust character controller dimensions
        controller.height = isCrouching ? 1.0f : 2.0f;
        controller.center = new Vector3(0, isCrouching ? 0.5f : 1.0f, 0);
        
        // Adjust movement speed
        speed = isCrouching ? (originalSpeed * 0.5f) : originalSpeed;
        
        Debug.Log(isCrouching ? "Crouching" : "Standing");
    }

    void TriggerJump()
    {
        Debug.Log($"TriggerJump called - IsGrounded: {isGrounded}, VerticalVelocity: {verticalVelocity}");
        
        if (isGrounded)
        {
            verticalVelocity = jumpForce;
            if (animator != null)
            {
                animator.SetTrigger("ToggleJump");
            }
            Debug.Log($"Jumping! New VerticalVelocity: {verticalVelocity}");
        }
        else
        {
            Debug.Log("Cannot jump - not grounded!");
        }
    }

    int CalculateFallDamage(float velocity)
    {
        float fallSpeed = Mathf.Abs(velocity);
        float excess = fallSpeed - Mathf.Abs(fallDamageThreshold);
        return Mathf.Clamp(Mathf.RoundToInt(excess * 50), 0, maxFallDamage);
    }

    void OnDrawGizmosSelected()
    {
        // Visualize ground check sphere in scene view
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}