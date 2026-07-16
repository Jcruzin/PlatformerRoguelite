using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class PowerUpTransitionSequence
{
    [Header("Frames")]
    public Sprite startFrame;
    public Sprite[] alternatingFrames;
    public Sprite finalFrame;

    [Header("Timing")]
    public float frameTime = 0.06f;
    public int loops = 3;
    public float finalHoldTime = 0.08f;

    public IEnumerator PlayStartFrame(PlayerSpriteController controller)
    {
        controller.SetSpriteOverride(startFrame);
        yield return new WaitForSecondsRealtime(frameTime);
    }

    public IEnumerator PlayLoops(PlayerSpriteController controller, bool lowerOpacity) 
    {
        for (int loop = 0; loop < loops; loop++)
        {
            for (int i = 0; i < alternatingFrames.Length; i++)
            {
                Sprite frame = alternatingFrames[i];
                if (frame == null) continue;
                if (lowerOpacity) controller.SetOpacity(0.7f);
                controller.SetSpriteOverride(frame);
                yield return new WaitForSecondsRealtime(frameTime);
                if (lowerOpacity) controller.SetOpacity(1.0f);
            }
        }
    }

    public IEnumerator PlayFinalFrame(PlayerSpriteController controller)
    {
        controller.SetSpriteOverride(finalFrame);
        yield return new WaitForSecondsRealtime(finalHoldTime);
    }
}

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerSpriteController))]
[RequireComponent(typeof(PlayerState))]
public class PlayerPowerupTransition : MonoBehaviour
{
    [Header("Big Mode Transition")]
    [SerializeField] private PowerUpTransitionSequence bigModeSequence;

    [Header("Back to Small Transition")]
    [SerializeField] private PowerUpTransitionSequence smallModeSequence;

    [Header("Midair Upgrade Boost")]
    [SerializeField] private float upwardVelocityThreshold = 0.1f;
    [SerializeField] private float midairUpgradeBoost = 1.25f;

    private PlayerMovement playerMovement;
    private PlayerState playerState;
    private PlayerSpriteController playerSpriteController;

    private bool isTransitioning;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerState = GetComponent<PlayerState>();
        playerSpriteController = GetComponent<PlayerSpriteController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (playerState.IsBigMode)
            {
                PlaySmallModeTransition();
            }
            else 
            {
                PlayBigModeTransition();
            }
        }
    }

    public void PlaySmallModeTransition()
    {
        if (isTransitioning || !playerState.IsBigMode) return;

        StartCoroutine(PlayTransition(
            smallModeSequence,
            applyPowerup: () => playerState.SetBigMode(false),
            true,
            false
        ));
    }

    public void PlayBigModeTransition()
    {
        if (isTransitioning || playerState.IsBigMode) return;

        StartCoroutine(PlayTransition(
            bigModeSequence,
            applyPowerup: () => playerState.SetBigMode(true),
            false, 
            true
        ));
    }

    private IEnumerator PlayTransition(
        PowerUpTransitionSequence sequence,
        System.Action applyPowerup,
        bool lowerOpacity,
        bool invokeBefore
    )
    {
        if (sequence == null)
        {
            yield break;
        }

        isTransitioning = true;

        Vector2 savedVelocity = playerMovement.Velocity;
        bool wasGrounded = playerMovement.IsGrounded;
        bool wasMovingUpward = !wasGrounded && savedVelocity.y > upwardVelocityThreshold;

        float previousTimeScale = Time.timeScale;

        playerMovement.SetControlLocked(true);

        if(invokeBefore) applyPowerup?.Invoke();

        Time.timeScale = 0f;

        if(sequence.startFrame != null) yield return sequence.PlayStartFrame(playerSpriteController);

        yield return sequence.PlayLoops(playerSpriteController, lowerOpacity);

        if (!invokeBefore) applyPowerup?.Invoke();

        if (wasMovingUpward)
        {
            Vector2 boostDirection = savedVelocity.sqrMagnitude > 0.001f
                ? savedVelocity.normalized : Vector2.up;
            savedVelocity += boostDirection * midairUpgradeBoost;
        }

        playerMovement.SetVelocity(savedVelocity);

        if(sequence.finalFrame != null) yield return sequence.PlayFinalFrame(playerSpriteController);

        Time.timeScale = previousTimeScale;

        playerSpriteController.ClearSpriteOverride();
        playerMovement.SetControlLocked(false);

        isTransitioning = false;
    }
}
