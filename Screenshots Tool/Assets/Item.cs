using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IPointerClickHandler
{
    public ItemTrigger leftTrigger;
    public ItemTrigger rightTrigger;
    
    private ScreenshotsPanelController _owner;
    private ScreenshotData _screenshotData;

    public void Init(ScreenshotsPanelController owner, ScreenshotData screenshotData) {
        _owner = owner;
        _screenshotData = screenshotData;
    }
    
    public ScreenshotData GetScreenshotData() {
        return _screenshotData;
    }
    
    public void SetTriggersActive(bool active) {
        leftTrigger.gameObject.SetActive(active);
        rightTrigger.gameObject.SetActive(active);
    }

    public void OnPointerClick(PointerEventData eventData) {
        _owner.StartMoveMode(this);
    }
}
