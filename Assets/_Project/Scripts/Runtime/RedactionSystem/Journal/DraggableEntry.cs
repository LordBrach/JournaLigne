using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    public bool isUsed = false;

    public String text;
    private GameObject _cloneInstance;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        _originalPosition = _rectTransform.position;
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
        CreateClone();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        _rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.alpha = 1f;
        Destroy(_cloneInstance);

        if (eventData.pointerDrag == null || eventData.pointerDrag.GetComponent<EntrySlot>() == null)
        {
            _canvasGroup.blocksRaycasts = true;
            _rectTransform.position = _originalPosition;
        }
        
    }
    
    private void CreateClone()
    {
        int originalIndex = transform.GetSiblingIndex();
        _cloneInstance = Instantiate(gameObject, transform.parent);
        _cloneInstance.transform.SetSiblingIndex(originalIndex);

        var cloneDraggable = _cloneInstance.GetComponent<DraggableEntry>();
        if (cloneDraggable != null)
        {
            Destroy(cloneDraggable);
        }

        var cloneCanvasGroup = _cloneInstance.GetComponent<CanvasGroup>();
        if (cloneCanvasGroup != null)
        {
            cloneCanvasGroup.alpha = 1f;
            cloneCanvasGroup.blocksRaycasts = false;
        }
    }
    
}
