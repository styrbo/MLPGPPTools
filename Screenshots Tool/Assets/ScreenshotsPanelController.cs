using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ScreenshotsPanelController : MonoBehaviour {
    
    public Item prefab;
    public Transform root;
    public Image cursorImage; // Reference to cursor image in scene
    
    private Item[] _items;
    private Item _selectedItem;
    private bool _inMoveMode = false;
    
    private void Start() {
        // Disable cursor image at start
        if (cursorImage != null) {
            cursorImage.gameObject.SetActive(false);
        }
        
        // Test data initialization
        InitializeTestData();
    }
    
    private void InitializeTestData() {
        // Create test data
        ScreenshotData[] testData = new ScreenshotData[5];
        for (int i = 0; i < testData.Length; i++) {
            testData[i] = new ScreenshotData { Name = "Screenshot_" + (i + 1) };
        }
        
        Debug.Log("=== INITIALIZED TEST DATA ===");
        LogScreenshotArray("Initial Data", testData);
        
        // Initialize with test data
        InitScreenshots(testData);
        
        Debug.Log("=== ITEMS ARRAY AFTER INIT ===");
        LogItemsArray();
    }
    
    private void LogScreenshotArray(string label, ScreenshotData[] data) {
        Debug.Log($"[{label}] Array Length: {data.Length}");
        for (int i = 0; i < data.Length; i++) {
            Debug.Log($"  [{i}] {data[i].Name}");
        }
    }
    
    private void LogItemsArray() {
        Debug.Log($"Items Array Length: {_items.Length}");
        for (int i = 0; i < _items.Length; i++) {
            Debug.Log($"  [{i}] {_items[i].GetScreenshotData().Name}");
        }
    }

    public void InitScreenshots(ScreenshotData[] data) {
        _items = new Item[data.Length];
        foreach (Transform child in root) {
            Destroy(child.gameObject);
        }

        for (var i = 0; i < data.Length; i++) {
            var screenshotData = data[i];
            var item = Instantiate(prefab, root);
            item.Init(this, screenshotData);
            _items[i] = item;
            
            // Subscribe to trigger events
            item.leftTrigger.onPointerEnter += (side) => OnTriggerHover(_items, item, side);
            item.rightTrigger.onPointerEnter += (side) => OnTriggerHover(_items, item, side);
        }
    }
    
    public void SetTriggersActive(bool active) {
        foreach (var item in _items) {
            item.SetTriggersActive(active);
        }
    }

    public void StartMoveMode(Item item) {
        _selectedItem = item;
        _inMoveMode = true;
        
        // Activate triggers for all items
        SetTriggersActive(true);
        
        // Enable cursor image
        if (cursorImage != null) {
            cursorImage.gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerHover(Item[] items, Item targetItem, TriggerSide side) {
        if (!_inMoveMode || _selectedItem == null) return;
        
        int selectedIndex = System.Array.IndexOf(items, _selectedItem);
        int targetIndex = System.Array.IndexOf(items, targetItem);
        
        if (selectedIndex == -1 || targetIndex == -1) return;
        
        // Move left (move before the target)
        if (side == TriggerSide.Left && selectedIndex != targetIndex) {
            MoveItem(items, selectedIndex, targetIndex);
        }
        // Move right (move after the target)
        else if (side == TriggerSide.Right) {
            int rightIndex = targetIndex + 1;
            // Clamp to valid range
            rightIndex = Mathf.Min(rightIndex, items.Length - 1);
            if (selectedIndex != rightIndex) {
                MoveItem(items, selectedIndex, rightIndex);
            }
        }
    }
    
    private void MoveItem(Item[] items, int fromIndex, int toIndex) {
        // Validate bounds
        if (fromIndex < 0 || fromIndex >= items.Length || toIndex < 0 || toIndex >= items.Length) {
            return;
        }
        
        if (fromIndex == toIndex) return;
        
        Item temp = items[fromIndex];
        
        // Shift items in the array
        if (fromIndex < toIndex) {
            // Moving right: shift items left
            for (int i = fromIndex; i < toIndex; i++) {
                items[i] = items[i + 1];
            }
        } else {
            // Moving left: shift items right
            for (int i = fromIndex; i > toIndex; i--) {
                items[i] = items[i - 1];
            }
        }
        
        items[toIndex] = temp;
        
        // Log the change
        Debug.Log($"=== MOVED ITEM ===");
        Debug.Log($"From Index: {fromIndex} → To Index: {toIndex}");
        LogItemsArray();
        
        // Update UI positions - reorder without destroying
        ReorderItemsInUI(items);
    }
    
    private void ReorderItemsInUI(Item[] items) {
        for (int i = 0; i < items.Length; i++) {
            items[i].transform.SetSiblingIndex(i);
        }
    }
    
    private void Update() {
        if (cursorImage != null && _inMoveMode) {
            // Follow cursor
            Vector2 mousePos = Mouse.current.position.ReadValue();
            cursorImage.rectTransform.position = mousePos;
        }
        
        // Exit move mode with escape or right-click
        if (_inMoveMode && (Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)) {
            EndMoveMode();
        }
    }
    
    private void EndMoveMode() {
        _inMoveMode = false;
        _selectedItem = null;
        SetTriggersActive(false);
        
        if (cursorImage != null) {
            cursorImage.gameObject.SetActive(false);
        }
    }
}