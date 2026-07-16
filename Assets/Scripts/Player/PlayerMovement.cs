using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Walk Movement")]
    [SerializeField] private float maxWalkSpeed = 7f;
    [SerializeField] private float maxRunSpeed = 13f;
    [SerializeField] private float midAirAcceleration = 4f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float turnDeceleration = 20f;
    [SerializeField] private float midAirDeceleration = 30f;

    [Header("Skid")]
    [SerializeField] private float skidSpeedThreshold = 2.5f;

    [Header("Jump")]
    [SerializeField] private float jumpHeightInTiles = 4f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    [SerializeField] private float lowJumpGravityMultiplier = 2.2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.08f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float jumpReleaseBufferTime = 0.1f;
    [SerializeField] private float runningJumpBoost = 0.25f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private float horizontalInput;
    private bool isSkidding;
    private bool isRunning = false;

    private bool isGrounded = true;
    private float jumpBufferCounter;
    private bool jumpHeld = false;
    private float jumpReleaseBufferCounter;
    private bool isJumping = false;

    private bool controlLocked = false;

    public Vector2 Velocity => rb.linearVelocity;
    public float HorizontalInput => horizontalInput;
    public bool IsSkidding => isSkidding;
    public bool IsRunning => isRunning;
    public bool IsGrounded => isGrounded;
    public float JumpBufferCounter => jumpBufferCounter;
    public bool JumpHeld => jumpHeld;
    public float JumpReleasedBufferCounter => jumpReleaseBufferCounter;
    public bool IsJumping => isJumping;

    public void SetControlLocked(bool locked)
    {
        controlLocked = locked;
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (controlLocked) return;
        jumpHeld = Input.GetKey(KeyCode.Z);
        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else if(jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            jumpReleaseBufferCounter = jumpReleaseBufferTime;
        }
        else if(jumpReleaseBufferCounter > 0f)
        {
            jumpReleaseBufferCounter -= Time.deltaTime;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.X);
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyJump();
        ApplyBetterGravity();
        if (isGrounded)
        {
            isJumping = false;
            ApplyGroundHorizontalMovement();
        }
        else
        {
            ApplyAirHorizontalMovement();
        }
    }

    private void ApplyGroundHorizontalMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        float currentAbsSpeed = Mathf.Abs(velocity.x);
        bool hasInput = horizontalInput != 0f;
        bool isTryingToReverse = hasInput &&
            Mathf.Sign(horizontalInput) != Mathf.Sign(velocity.x) &&
            currentAbsSpeed >= skidSpeedThreshold;

        bool shouldSlowToWalkSpeed =
            hasInput &&
            !isRunning &&
            currentAbsSpeed >= maxWalkSpeed &&
            !isTryingToReverse;

        isSkidding = isTryingToReverse;

        if (hasInput)
        {
            float maxSpeed = isRunning ? maxRunSpeed : maxWalkSpeed;
            float targetSpeed = horizontalInput * maxSpeed;
            float accelerationRate = isTryingToReverse ? turnDeceleration: shouldSlowToWalkSpeed ? deceleration : acceleration;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetSpeed,
                accelerationRate * Time.fixedDeltaTime
            );
        }
        else
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = velocity;
    }

    private void ApplyAirHorizontalMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        bool hasInput = horizontalInput != 0f;
        float currentAbsSpeed = Mathf.Abs(velocity.x);
        if (hasInput)
        {
            bool isMovingHorizontally = currentAbsSpeed >= 0.01f;

            bool inputMatchesVelocity = 
                !isMovingHorizontally ||
                Mathf.Sign(horizontalInput) == Mathf.Sign(velocity.x);

            if (!inputMatchesVelocity)
            {
                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    0f,
                    midAirDeceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                float maxSpeed = isRunning ? maxRunSpeed : maxWalkSpeed;
                float targetSpeed = horizontalInput * maxSpeed;

                velocity.x = Mathf.MoveTowards(
                    velocity.x,
                    targetSpeed,
                    midAirAcceleration * Time.fixedDeltaTime
                );
            }
        }

        rb.linearVelocity = velocity;
    }

    private void UpdateFacingDirection()
    {
        if (horizontalInput < 0f && isGrounded)
        {
            sprite.flipX = true;
        }
        else if (horizontalInput > 0f && isGrounded)
        {
            sprite.flipX = false;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void ApplyJump()
    {
        if(jumpBufferCounter > 0f && isGrounded)
        {
            Vector2 velocity = rb.linearVelocity;
            float speedRatio = Mathf.InverseLerp(maxWalkSpeed, maxRunSpeed, Mathf.Abs(rb.linearVelocity.x));
            float jumpBoost = runningJumpBoost * speedRatio;
            velocity.y = (jumpHeightInTiles * 4f) + jumpBoost;
            rb.linearVelocity = velocity;
            isGrounded = false;
            jumpBufferCounter = 0;
            isJumping = true;
        }

        if(jumpReleaseBufferCounter > 0f && rb.linearVelocity.y > 0f)
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y *= jumpCutMultiplier;
            rb.linearVelocity = velocity;
            jumpReleaseBufferCounter = 0;
        }
    }

    private void ApplyBetterGravity()
    {
        Vector2 velocity = rb.linearVelocity;

        if(velocity.y < 0f)
        {
            velocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0f && !jumpHeld)
        {
            velocity += Vector2.up * Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        rb.linearVelocity = velocity;
    }
}
