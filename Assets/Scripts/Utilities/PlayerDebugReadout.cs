using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerDebugReadout : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private bool showDebug = true;

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

        Vector2 velocity = playerMovement.Velocity;

        string debugText =
            $"VX: {velocity.x:0.00}\n" + 
            $"VY: {velocity.y:0.00}\n" + 
            $"Input: {playerMovement.HorizontalInput:0}\n" +
            $"Is skidding: {playerMovement.IsSkidding}\n" + 
            $"Is running: {playerMovement.IsRunning}\n";

        GUI.Label(new Rect(12, 12, 120, 120), debugText, style);
    }
}
