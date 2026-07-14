using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(PlayerMovement))]
public class PlayerSpriteController : MonoBehaviour
{

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] movementFrames;
    [SerializeField] private Sprite skidSprite;
    [SerializeField] private Sprite jumpSprite;

    [Header("Timing")]
    [SerializeField] private float minimumSkidTime = 0.12f;

    [Header("Movement")]
    [SerializeField] private float movementAnimationThreshold = 0.1f;
    [SerializeField] private float maxExpectedSpeed = 13f;
    [Tooltip("Seconds per frame at very low movement speed")]
    [SerializeField] private float slowestFrameTime = 0.16f;
    [Tooltip("Seconds per frame at max movement speed")]
    [SerializeField] private float fastestFrameTime = 0.055f;

    private SpriteRenderer spriteRenderer;
    private PlayerMovement playerMovement;

    private float skidTimer;
    private float movementFrameTimer;
    private int movementFrameIndex;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();

        if(idleSprite == null )
        {
            idleSprite = spriteRenderer.sprite;
        }
    }

    private void LateUpdate()
    {
        UpdateSkidTimer();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (skidTimer > 0f && skidSprite != null)
        {
            spriteRenderer.sprite = skidSprite;
            return;
        }

        if (playerMovement.IsJumping && jumpSprite != null)
        {
            spriteRenderer.sprite = jumpSprite;
            return;
        }

        float horizontalSpeed = Mathf.Abs(playerMovement.Velocity.x);

        bool shouldPlayMovementAnimation =
            playerMovement.IsGrounded &&
            horizontalSpeed >= movementAnimationThreshold &&
            movementFrames != null &&
            movementFrames.Length > 0;

        if (shouldPlayMovementAnimation)
        {
            PlayMovementAnimation(horizontalSpeed);
            return;
        }

        ResetMovementAnimation();

        if (idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    private void UpdateSkidTimer()
    {
        if (playerMovement.IsSkidding)
        {
            skidTimer = minimumSkidTime;
        }

        if(skidTimer > 0f)
        {
            skidTimer -= Time.deltaTime;
        }
    }

    private void PlayMovementAnimation(float horizontalSpeed)
    {
        float speedPercent = Mathf.InverseLerp(
            0f,
            maxExpectedSpeed,
            horizontalSpeed
        );

        float currentFrameTime = Mathf.Lerp(
            slowestFrameTime,
            fastestFrameTime,
            speedPercent
        );

        movementFrameTimer += Time.deltaTime;

        if(movementFrameTimer >= currentFrameTime)
        {
            movementFrameTimer = 0f;
            movementFrameIndex = (movementFrameIndex + 1) % movementFrames.Length;
;       }

        spriteRenderer.sprite = movementFrames[movementFrameIndex];
    }

    private void ResetMovementAnimation()
    {
        movementFrameTimer = 0f;
        movementFrameIndex = 0;
    }
}
