using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour
{
    [Header("Item Info")]
    [SerializeField] private string clusterId;
    [SerializeField] private Item item;

    public Item Item => item;
    
    private bool isInitialized;
    
    // TODO: reference to who dropped item.

    public void Setup(string clusterId, Item item, Vector3 dropPosition)
    {
        this.clusterId = clusterId;
        this.item = item;
        transform.position = dropPosition;

        if (!isInitialized)
            SetupVisuals();
        
        isInitialized = true;
    }

    private void SetupVisuals()
    {
        var visual = Instantiate(item.ItemConfig.ItemVisualPrefab, transform);
        var angle = Random.Range(0f, 360f);
        visual.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

        var visualRenderer = visual.GetComponent<MeshRenderer>();
        if (visualRenderer != null)
        {
            var height = visualRenderer.bounds.size.y;
            visual.transform.localPosition = new Vector3(0f, height / 2f, 0f);
        }
    }
}
