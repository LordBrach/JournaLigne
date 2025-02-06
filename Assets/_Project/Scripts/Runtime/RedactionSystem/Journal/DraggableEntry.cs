using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableEntry : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _canvas;
    [HideInInspector] public RectTransform rectTransform;
    [HideInInspector] public CanvasGroup canvasGroup;
    [HideInInspector] public Transform originalPosition;
    [HideInInspector] public bool isUsed = false;

    public String text;
    private TextMeshProUGUI _textMesh;
    
    [HideInInspector] public GameObject cloneInstance;
    private Transform _cloneTransform;
    
    private Animator _animator;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponentInParent<Canvas>();
        _animator = GetComponentInParent<Animator>();
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        StartDrag(eventData);
    }

    public void StartDrag(PointerEventData eventData)
    {
        CreateClone();
        originalPosition = rectTransform.parent;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        originalPosition.position = transform.parent.position;
        transform.SetParent(_canvas.transform, true);
        
        _animator.SetBool("OpenClose", false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        UpdateDrag(eventData);
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isUsed) return;
        
        ResetPosition();
    }

    public void EndDrag()
    {
        StopDrag();
    }

    public void StopDrag()
    {
        ResetPosition();
        
        if (isUsed) return;
        _animator.SetBool("OpenClose", true);
    }

    public void ResetPosition()
    {
        
        ApplyStrikeThrough();
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
    
    public void ApplyStrikeThrough()
    {
        if (isUsed)
            _textMesh.fontStyle |= FontStyles.Strikethrough;
        else
            _textMesh.fontStyle &= ~FontStyles.Strikethrough;
    }
}
