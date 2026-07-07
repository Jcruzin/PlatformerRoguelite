using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class GroundStripBuilder : MonoBehaviour
{
    [Header("Tile Settings")]
    [SerializeField] private Sprite groundSprite;
    [SerializeField] private int tileCount = 16;
    [SerializeField] private float yPos = -6f;
    [SerializeField] private int sortingOrder = 0;

    [Header("Generation Settings")]
    [SerializeField] private string generatedTilePrefix = "GroundTile_";

    [ContextMenu("Generate Ground Strip")]
    public void GenerateGroundStrip()
    {
        if(groundSprite == null)
        {
            Debug.LogWarning("No Groundstrip sprite assigned");
            return;
        }

        ClearGeneratedTiles();

        float startX = -(tileCount) / 2f;

        for (int i = 0; i < tileCount; i++) 
        {
            GameObject tile = new GameObject($"{generatedTilePrefix}{i:00}");

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(tile, "Generate Ground Tile");
#endif
            tile.transform.SetParent(transform);
            tile.transform.localPosition = new Vector3(startX + i, yPos, 0f);
            tile.transform.localRotation = Quaternion.identity;
            tile.transform.localScale = Vector3.one;

            SpriteRenderer spriteRenderer = tile.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = groundSprite;
            spriteRenderer.sortingOrder = sortingOrder;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }

    [ContextMenu("Clear Generated Tiles")]
    public void ClearGeneratedTiles()
    {
        for(int i = transform.childCount - 1;  i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (!child.name.StartsWith(generatedTilePrefix))
            {
                continue;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
            else
            {
                Destroy(child.gameObject);
            }
#else
            Destroy(child.gameObject);
#endif
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}
