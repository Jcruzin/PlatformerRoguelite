using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Walk Movement")]
    [SerializeField] private float maxWalkSpeed = 7f;
    [SerializeField] private float maxRunSpeed = 13f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float turnDeceleration = 20f;

    [Header("Skid")]
    [SerializeField] private float skidSpeedThreshold = 2.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private float horizontalInput;
    private bool isSkidding;
    private bool isRunning = false;

    public Vector2 Velocity => rb.linearVelocity;
    public float HorizontalInput => horizontalInput;
    public bool IsSkidding => isSkidding;
    public bool IsRunning => isRunning;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isRunning = Input.GetKey(KeyCode.X);
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void ApplyHorizontalMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        float currentVelocity = Mathf.Abs(velocity.x);
        bool slowingDown = !isRunning && currentVelocity > maxWalkSpeed;
        float targetSpeed = isRunning ? horizontalInput * maxRunSpeed : horizontalInput * maxWalkSpeed;

        bool hasInput = horizontalInput != 0f;
        bool isTryingToReverse = hasInput &&
            Mathf.Sign(horizontalInput) != Mathf.Sign(velocity.x) &&
            Mathf.Abs(velocity.x) >= skidSpeedThreshold;

        isSkidding = isTryingToReverse;

        if (hasInput)
        {
            float accelerationRate = isTryingToReverse ? turnDeceleration: slowingDown ? deceleration : acceleration;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetSpeed,
                accelerationRate * Time.fixedDeltaTime
            );
        }
        else
        {
            float currentAbsSpeed = Mathf.Abs(velocity.x);
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = velocity;
    }

    private void UpdateFacingDirection()
    {
        if (horizontalInput < 0f)
        {
            sprite.flipX = true;
        }
        else if (horizontalInput > 0f)
        {
            sprite.flipX = false;
        }
    }
}
