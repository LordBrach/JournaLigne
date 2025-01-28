using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntrySlot : MonoBehaviour, IDropHandler
{
    private TextMeshProUGUI _textMesh;
    private NotebookEntry _notebookEntry;

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            DraggableEntry draggableEntry = eventData.pointerDrag.GetComponent<DraggableEntry>();
            if (draggableEntry && _textMesh)
            {
                _notebookEntry = NoteBook.instance.GetEntry(draggableEntry.text);
                
                if(_notebookEntry != null)
                    _textMesh.text = _notebookEntry.associatedText;
            }
        }
    }
    
}
