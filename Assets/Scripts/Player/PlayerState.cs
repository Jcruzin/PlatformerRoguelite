using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerState : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;

    private BoxCollider2D boxCollider;

    private bool isBigMode = false;

    public bool IsBigMode => isBigMode;

    public void SetBigMode(bool big)
    {
        if(isBigMode == big) return;
        ChangeSize();
    }

    private void Awake()
    {
       boxCollider = GetComponent<BoxCollider2D>();
    }

    private void ChangeSize()
    {
        if (!isBigMode)
        {
            isBigMode = true;
            transform.position += new Vector3(0f, 0.5f, 0f);
            boxCollider.size *= new Vector3(1f, 2f, 1f);
            groundCheck.localPosition -= new Vector3(0f, 0.5f, 0f);
        }
        else
        {
            isBigMode = false;
            transform.position -= new Vector3(0f, 0.5f, 0f);
            boxCollider.size *= new Vector3(1f, 0.5f, 1f);
            groundCheck.localPosition += new Vector3(0f, 0.5f, 0f);
        }
    }
}
