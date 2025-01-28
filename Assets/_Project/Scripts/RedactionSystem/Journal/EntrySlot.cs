using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntrySlot : MonoBehaviour, IDropHandler
{
    private TextMeshProUGUI _textMesh;
    public NotebookEntry NotebookEntry { get; private set; }

    private void Start()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition3D = GetComponent<RectTransform>().anchoredPosition;
            
            DraggableEntry draggableEntry = eventData.pointerDrag.GetComponent<DraggableEntry>();
            if (draggableEntry != null)
            {
                NotebookEntry = NoteBook.instance.GetEntry(draggableEntry.TextMesh.text);
                _textMesh.text = NotebookEntry.associatedText;
            }
        }
    }
}
