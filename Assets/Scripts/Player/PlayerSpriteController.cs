using System;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
public class PlayerAnimationSet
{
    public Sprite idle;
    public Sprite[] movement;
    public Sprite jump;
    public Sprite skid;
}

[RequireComponent (typeof(SpriteRenderer))]
[RequireComponent (typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerState))]
public class PlayerSpriteController : MonoBehaviour
{

    [Header("Sprites")]
    [SerializeField] private PlayerAnimationSet bigAnimations;
    [SerializeField] private PlayerAnimationSet smallAnimations;

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
    private PlayerState playerState;
    private PlayerAnimationSet playerAnimationSet;

    private float skidTimer;
    private float movementFrameTimer;
    private int movementFrameIndex;
    private bool hasSpriteOverride;
    private Sprite overrideSprite;

    public void SetSpriteOverride(Sprite sprite)
    {
        hasSpriteOverride = true;
        overrideSprite = sprite;
        spriteRenderer.sprite = sprite;
    }

    public void ClearSpriteOverride()
    {
        hasSpriteOverride = false;
        overrideSprite = null;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerState = GetComponent<PlayerState>();
        playerAnimationSet = playerState.IsBigMode ? bigAnimations : smallAnimations;
    }

    private void LateUpdate()
    {
        if (hasSpriteOverride)
        {
            spriteRenderer.sprite = overrideSprite;
            return;
        }
        playerAnimationSet = playerState.IsBigMode ? bigAnimations : smallAnimations;
        UpdateSkidTimer();
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (skidTimer > 0f && playerAnimationSet.skid != null)
        {
            spriteRenderer.sprite = playerAnimationSet.skid;
            return;
        }

        if (playerMovement.IsJumping && playerAnimationSet.jump != null)
        {
            spriteRenderer.sprite = playerAnimationSet.jump;
            return;
        }

        float horizontalSpeed = Mathf.Abs(playerMovement.Velocity.x);

        bool shouldPlayMovementAnimation =
            playerMovement.IsGrounded &&
            horizontalSpeed >= movementAnimationThreshold &&
            playerAnimationSet.movement != null &&
            playerAnimationSet.movement.Length > 0;

        if (shouldPlayMovementAnimation)
        {
            PlayMovementAnimation(horizontalSpeed);
            return;
        }

        ResetMovementAnimation();

        if (playerAnimationSet.idle != null)
        {
            spriteRenderer.sprite = playerAnimationSet.idle;
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
            movementFrameIndex = (movementFrameIndex + 1) % playerAnimationSet.movement.Length;
;       }

        spriteRenderer.sprite = playerAnimationSet.movement[movementFrameIndex];
    }

    private void ResetMovementAnimation()
    {
        movementFrameTimer = 0f;
        movementFrameIndex = 0;
    }

    public void SetOpacity(float alphaValue)
    {
        Color currentColor = spriteRenderer.color;
        currentColor.a = alphaValue;
        spriteRenderer.color = currentColor;
    }
}
