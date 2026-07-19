using UnityEngine;

public class PauseInputController : MonoBehaviour
{
    [SerializeField] private float pauseBuffer = 0.1f;
    private float inputTimer = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && inputTimer <= 0)
        {
            if (GamePauseManager.IsPausedFor(PauseReason.PauseMenu))
            {
                GamePauseManager.ReleasePause(PauseReason.PauseMenu);
                inputTimer = pauseBuffer;
            }
            else
            {
                GamePauseManager.RequestPause(PauseReason.PauseMenu);
                inputTimer = pauseBuffer;
            }
        }

        if (inputTimer > 0)
        {
            inputTimer -= Time.unscaledDeltaTime;
        }
    }
}
