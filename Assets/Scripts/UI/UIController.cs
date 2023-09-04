using UnityEngine;

public class UIController : ControllerBase
{
    [SerializeField] private GameObject blurBackground;
    [SerializeField] private Canvas uiCanvas;
    
    // TODO: all views derive from UIViewBase, load them on initialization.
    [Header("Views")] 
    [SerializeField] private InventoryView inventoryView;

    public Canvas UICanvas => uiCanvas;

    protected override void StartController()
    {
        base.StartController();
        
        InitViews();
    }

    private void InitViews()
    {
        if (inventoryView != null)
        {
            inventoryView.Init(this, gameController);
        }
    }

    private void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryView.IsOpen)
            {
                inventoryView.Close();
            }
            else
            {
                inventoryView.Open();
            }
        }
    }

    // TODO: change to list of BaseView's
    public bool IsAnyViewShown()
    {
        return inventoryView.IsOpen;
    }
    
    public void SetBlurState(bool state)
    {
        blurBackground.SetActive(state);
    }
}
