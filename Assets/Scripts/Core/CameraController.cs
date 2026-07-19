using UnityEngine;

[RequireComponent (typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Player Position")]
    [SerializeField] private Transform playerTarget;

    [Tooltip("0 Represents the center of the screen")]
    [SerializeField] private float scrollOffset = 0f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useRightLimit = false;
    [SerializeField] private float rightLimit = 100f;

    [Header("Left Boundary Wall")]
    [SerializeField] private BoxCollider2D leftBoundary;

    private Camera cameraComp;
    private float minimumCameraX;
    private float lockedY;
    private float lockedZ;

    private void Awake()
    {
        cameraComp = GetComponent<Camera>();

        Vector3 startingPos = transform.position;

        minimumCameraX = startingPos.x;
        lockedY = startingPos.y;
        lockedZ = startingPos.z;
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
        UpdateLeftBoundary();
    }

    private void UpdateCameraPosition()
    {
        if (playerTarget == null)
        {
            Debug.LogError("Player not assigned to camera");
            return;
        }

        float desiredCameraX = playerTarget.position.x - scrollOffset;

        float nextCameraX = Mathf.Max(
            transform.position.x,
            desiredCameraX,
            minimumCameraX
        );

        if (useRightLimit)
        {
            nextCameraX = Mathf.Min(nextCameraX, rightLimit);
        }

        transform.position = new Vector3(
            nextCameraX,
            lockedY,
            lockedZ
        );
    }

    private void UpdateLeftBoundary()
    {
        if(leftBoundary == null)
        {
            return;
        }

        float halfHeight = cameraComp.orthographicSize;
        float halfWidth = halfHeight * cameraComp.aspect;

        float cameraLeftEdgeX = transform.position.x - halfWidth;

        Transform wallTransform = leftBoundary.transform;

        wallTransform.position = new Vector3(
            cameraLeftEdgeX - 0.6f,
            lockedY,
            wallTransform.position.z
        );
    }
}
