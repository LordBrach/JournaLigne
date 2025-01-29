using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;

    public String text;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalPosition = _rectTransform.position;
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;

        if (eventData.pointerDrag == null || eventData.pointerDrag.GetComponent<EntrySlot>() == null)
        {
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.position = _originalPosition;
        }
        
    }
    
}
