using System;
using UnityEngine;

[ExecuteAlways]
public class VelocityGizom : MonoBehaviour
{
    [SerializeField] private Rigidbody2D targetRigidBody;
    [SerializeField] private float arrowScale = 0.35f;
    [SerializeField] private bool drawOnlyWhenPlaying = true;
    [SerializeField] private bool showGizmo = true;

    private void OnDrawGizmos()
    {
        if(targetRigidBody == null || !showGizmo)
        {
            return;
        }

        if (drawOnlyWhenPlaying && !Application.isPlaying) 
        {
            return;
        }

        Vector2 velocity = targetRigidBody.linearVelocity;

        Vector3 start = targetRigidBody.transform.position;
        Vector3 end = start + new Vector3(velocity.x, velocity.y, 0f) * arrowScale;

        Gizmos.DrawLine(start, end);
        DrawArrowHead(start, end);
    }

    private void DrawArrowHead(Vector3 start, Vector3 end) 
    {
        Vector3 direction = end - start;

        if(direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        direction.Normalize();

        Vector3 right = Quaternion.Euler(0f, 0f, 150f) * direction;
        Vector3 left = Quaternion.Euler(0f, 0f, -150f) * direction;

        float arrowHeadLength = 0.5f;

        Gizmos.DrawLine(end, end + right * arrowHeadLength);
        Gizmos.DrawLine(end, end + left * arrowHeadLength);
    }
}
