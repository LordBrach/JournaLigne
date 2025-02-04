using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Canvas _canvas;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public Transform originalPosition;
    public bool isUsed = false;

    public String text;
    public GameObject cloneInstance;
    private Transform _cloneTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StartDrag(eventData);
    }

    public void StartDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        CreateClone();
        originalPosition = rectTransform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        originalPosition.position = transform.parent.position;
        transform.SetParent(_canvas.transform, true); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateDrag(eventData);
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void EndDrag()
    {
        if (isUsed) return;
        
        ResetPosition();
    }

    public void ResetPosition()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        transform.SetParent(_cloneTransform, true);
        rectTransform.anchoredPosition = originalPosition.position;
        Destroy(cloneInstance);
    }
    
    public void CreateClone()
    {
        int originalIndex = transform.GetSiblingIndex();
        cloneInstance = Instantiate(gameObject, transform.parent);
        cloneInstance.transform.SetSiblingIndex(originalIndex);
        _cloneTransform = cloneInstance.transform.parent;

        var cloneDraggable = cloneInstance.GetComponent<DraggableEntry>();
        cloneDraggable.canvasGroup.blocksRaycasts = false;

        var cloneCanvasGroup = cloneInstance.GetComponent<CanvasGroup>();
        if (cloneCanvasGroup != null)
        {
            cloneCanvasGroup.alpha = 1f;
            cloneCanvasGroup.blocksRaycasts = false;
        }
    }
    
}
