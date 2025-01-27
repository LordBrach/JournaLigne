using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        _rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnDrop(PointerEventData eventData)
    {
        transform.SetParent(_originalParent);
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
}
