using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntrySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private TextMeshProUGUI _textMesh;
    private NotebookEntry _notebookEntry;
    private DraggableEntry _draggableEntry;
    private JournalBlock _multiplier;
    private Journal _journal; 

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _multiplier = GetComponentInChildren<JournalBlock>();
        _journal = GetComponentInParent<Journal>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_draggableEntry) return;
        
        _draggableEntry.isUsed = false;
        _draggableEntry.canvasGroup.blocksRaycasts = true;
        _draggableEntry.StartDrag(eventData);
        
        _textMesh.text = "";
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_draggableEntry) return;
        
        _draggableEntry.UpdateDrag(eventData);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_draggableEntry) return;
        
        _draggableEntry.EndDrag();

        _notebookEntry = null;
        _draggableEntry = null;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            _draggableEntry = eventData.pointerDrag.GetComponent<DraggableEntry>();
            if (_draggableEntry)
            {
                DropEvent();
                return;
            }

            EntrySlot entrySlot = eventData.pointerDrag.GetComponent<EntrySlot>();
            if (entrySlot)
            {
                _draggableEntry = entrySlot._draggableEntry;
                DropEvent();
            }
        }
    }

    public void DropEvent()
    {
        if (_draggableEntry && !_draggableEntry.isUsed && _textMesh)
        {
            Debug.Log("OnDrop2");
            _notebookEntry = NoteBook.instance.GetEntry(_draggableEntry.text);

            if (_notebookEntry != null)
            {
                _textMesh.text = _notebookEntry.associatedText;
                _draggableEntry.EndDrag();
                _draggableEntry.isUsed = true;
            }

            if (!_journal || !_multiplier || _notebookEntry == null) return;
                
            _journal.AddAppreciation(Factions.Government, _notebookEntry.government, _multiplier.multiplier);
            _journal.AddAppreciation(Factions.Rebels, _notebookEntry.rebels, _multiplier.multiplier);
            _journal.AddAppreciation(Factions.People, _notebookEntry.people, _multiplier.multiplier);
        }   
    }
    
}
