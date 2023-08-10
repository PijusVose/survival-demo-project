using UnityEngine;

public class ItemsPooler : MonoBehaviour
{
    // Items are spawned in clusters.
    // When dropping item, check for closest cluster, if too far away, create new cluster
    [SerializeField] private Transform itemDropPrefab;
    [SerializeField] private LayerMask groundLayerMask;
    
    public void DropItem(Item itemData)
    {
        
    }

    private Vector3 GetDropPosition(Vector3 startPosition)
    {
        if (Physics.Raycast(startPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            return hit.point;
        }

        return startPosition;
    }
}
