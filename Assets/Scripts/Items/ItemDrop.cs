using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemDrop : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    [SerializeField] private string clusterId;
    [SerializeField] private Item item;

    [Header("Interactable")] 
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private string interactActionName;

    public string DropId => dropId;
    public Item Item => item;
    public Vector3 PromptOffset => promptOffset;
    public KeyCode InteractKey => interactKey;
    public string InteractActionName => interactActionName;

    private string dropId;
    private ItemsController itemsController;
    private bool isInitialized;
    
    // TODO: reference to who dropped item.

    public void Setup(string clusterId, Item item, Vector3 dropPosition)
    {
        dropId = Guid.NewGuid().ToString();
        this.clusterId = clusterId;
        this.item = item;
        transform.position = dropPosition;

        if (!isInitialized)
            SetupVisuals();

        if (itemsController == null)
            itemsController = GameController.Instance.GetController<ItemsController>();
        
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
    
    public void OnInteract()
    {
        itemsController.PickupItem(this);
    }
}
