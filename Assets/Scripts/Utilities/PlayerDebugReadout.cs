using UnityEngine;

public class PlayerDebugReadout : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private bool showDebug = true;
    [SerializeField] private bool showRunningDebug = true;
    [SerializeField] private bool showJumpingDebug = true;

    private GUIStyle style;

    private void Awake()
    {
        style = new GUIStyle
        {
            fontSize = 17,
            normal = 
            {
                textColor = Color.white,
            }
        };
    }

    private void OnGUI()
    {
        if(!showDebug || playerMovement == null)
        {
            return;
        }

        string debugText = "";
        if (showRunningDebug)
        {
            Vector2 velocity = playerMovement.Velocity;

            debugText =
                $"Big Mode: {playerState.IsBigMode}\n" +
                $"VX: {velocity.x:0.00}\n" +
                $"VY: {velocity.y:0.00}\n" +
                $"Input: {playerMovement.HorizontalInput:0}\n" +
                $"Is skidding: {playerMovement.IsSkidding}\n" +
                $"Is running: {playerMovement.IsRunning}\n";
        }
        else if(showJumpingDebug)
        {
            debugText =
                $"Big Mode: {playerState.IsBigMode}\n" +
                $"isCrouched: {playerMovement.IsCrouched}\n" +
                $"isGrounded: {playerMovement.IsGrounded}\n" +
                $"jumpBufferCounter: {playerMovement.JumpBufferCounter:0.00}\n" +
                $"jumpHeld: {playerMovement.JumpHeld}\n" +
                $"jumpReleased: {playerMovement.JumpReleasedBufferCounter:0.00}\n" +
                $"isJumping: {playerMovement.IsJumping}\n";
        }

        GUI.Label(new Rect(12, 12, 120, 120), debugText, style);
    }
}
