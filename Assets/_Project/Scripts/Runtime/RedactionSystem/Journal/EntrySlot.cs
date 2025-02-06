using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EntrySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private TextMeshProUGUI _textMesh;
    private NotebookEntry _notebookEntry;
    private DraggableEntry _draggableEntry;
    private JournalBlock _multiplier;
    
    [SerializeField] private Sprite rebelsIcon;
    [SerializeField] private Sprite governmentIcon;
    [SerializeField] private Image icon;
    
    [HideInInspector] public float currentAppreciation;
    [HideInInspector] public bool haveAppreciation = false;

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _multiplier = GetComponentInParent<JournalBlock>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_draggableEntry == null) return;

        _draggableEntry.isUsed = false;
        _draggableEntry.canvasGroup.blocksRaycasts = false;

        _draggableEntry.transform.position = gameObject.transform.position;
        
        _draggableEntry.StartDrag(eventData);
        _textMesh.text = "";
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggableEntry == null) return;
        _draggableEntry.UpdateDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggableEntry == null) return;

        _draggableEntry.EndDrag();
        _draggableEntry.canvasGroup.blocksRaycasts = true;
        
        icon.gameObject.SetActive(false);
        haveAppreciation = false;
        
        _notebookEntry = null;
        _draggableEntry = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableEntry draggedEntry = eventData.pointerDrag.GetComponent<DraggableEntry>();

        if (draggedEntry)
        {
            if (_draggableEntry)
            {
                _draggableEntry.isUsed = false;
                _draggableEntry.ApplyStrikeThrough();
                _draggableEntry.ResetPosition();
            }
                
            _draggableEntry = draggedEntry;
            DropEvent();
            return;
        }

        EntrySlot draggedSlot = eventData.pointerDrag.GetComponent<EntrySlot>();
        if (draggedSlot != null && draggedSlot._draggableEntry != null)
        {
            if (_draggableEntry)
            {
                _draggableEntry.isUsed = false;
                _draggableEntry.ApplyStrikeThrough();
                _draggableEntry.ResetPosition();
            }
            
            _draggableEntry = draggedSlot._draggableEntry;
            DropEvent();
        }
    }

    private void DropEvent()
    {
        if (_draggableEntry == null || _draggableEntry.isUsed || _textMesh == null) return;
        
        _notebookEntry = NoteBook.instance.GetEntry(_draggableEntry.text);
        if (_notebookEntry != null)
        {
            _textMesh.text = _notebookEntry.associatedText;
            _draggableEntry.EndDrag();
            _draggableEntry.isUsed = true;
            haveAppreciation = true;
        }
        
        if (_multiplier == null || _notebookEntry == null) return;
        
        currentAppreciation = _notebookEntry.people * _multiplier.multiplier;
        
        if (currentAppreciation > 0)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = rebelsIcon;
        }
        else if (currentAppreciation < 0)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = governmentIcon;
        }
    }
    
}
