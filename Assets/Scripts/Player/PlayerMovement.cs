using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Walk Movement")]
    [SerializeField] private float maxWalkSpeed = 6f;
    [SerializeField] private float walkAcceleration = 30f;
    [SerializeField] private float walkDeceleration = 10f;
    [SerializeField] private float turnDeceleration = 18f;

    [Header("Skid")]
    [SerializeField] private float skidSpeedThreshold = 2.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private float horizontalInput;
    private bool isSkidding;

    public Vector2 Velocity => rb.linearVelocity;
    public float HorizontalInput => horizontalInput;
    public bool IsSkidding => isSkidding;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = rb.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        UpdateFacingDirection();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void ApplyHorizontalMovement()
    {
        Vector2 velocity = rb.linearVelocity;
        float targetSpeed = horizontalInput * maxWalkSpeed;

        bool hasInput = horizontalInput != 0f;
        bool isTryingToReverse = hasInput &&
            Mathf.Sign(horizontalInput) != Mathf.Sign(velocity.x) &&
            Math.Abs(velocity.x) >= skidSpeedThreshold;

        isSkidding = isTryingToReverse;

        if (hasInput)
        {
            float acclerationRate = isTryingToReverse ? turnDeceleration : walkAcceleration;
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                targetSpeed,
                acclerationRate * Time.fixedDeltaTime
            );
        }
        else
        {
            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                walkDeceleration * Time.fixedDeltaTime
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
