using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerSpriteController))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerDeathSequence : MonoBehaviour
{
    [Header("Death")]
    [SerializeField] private Sprite dead;
    [SerializeField] private float hopDelay = 0.15f;
    [SerializeField] private float hopVelocity = 8f;
    [SerializeField] private float gravity = 24f;
    [SerializeField] private float fallBelowCameraPadding = 2f;
    [SerializeField] private float maxDeathAnimationTime = 3f;

    public event Action DeathAnimationFinished;

    private PlayerMovement playerMovement;
    private PlayerSpriteController spriteController;
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    private bool isPlaying = false;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        spriteController = GetComponent<PlayerSpriteController>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    public void PlayDeath()
    {
        if (isPlaying)
        {
            return;
        }

        StartCoroutine(PlayDeathRoutine());
    }

    private IEnumerator PlayDeathRoutine()
    {
        isPlaying = true;

        GamePauseManager.RequestPause(PauseReason.GameOver);
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        bc.enabled = false;
      
        if (dead != null)
        {
            spriteController.SetSpriteOverride(dead);
        }
        
        yield return new WaitForSecondsRealtime(hopDelay);
  
        float verticalVelocity = hopVelocity;
        float elapsed = 0;

        while(elapsed < maxDeathAnimationTime)
        {
            float delta = Time.unscaledDeltaTime;

            Vector3 position = transform.position;
            position.y += verticalVelocity * delta;
            transform.position = position;

            verticalVelocity -= gravity * delta;
            elapsed += delta;

            if (HasFallenBelowCamera())
            {
                break;
            }

            yield return null;
        }

        DeathAnimationFinished?.Invoke();

        GamePauseManager.ReleasePause(PauseReason.GameOver);

        isPlaying = false;
    }

    private bool HasFallenBelowCamera()
    {
        Camera camera = Camera.main;

        if(camera == null)
        {
            return false;
        }

        float cameraBottomY =
            camera.transform.position.y - camera.orthographicSize;

        return transform.position.y < cameraBottomY - fallBelowCameraPadding;
    }
}
