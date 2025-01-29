using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntrySlot : MonoBehaviour, IDropHandler
{
    private TextMeshProUGUI _textMesh;
    private NotebookEntry _notebookEntry;
    private JournalBlock _multiplier;
    private Journal _journal; 

    private void Awake()
    {
        _textMesh = GetComponentInChildren<TextMeshProUGUI>();
        _multiplier = GetComponentInChildren<JournalBlock>();
        _journal = GetComponentInParent<Journal>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

            DraggableEntry draggableEntry = eventData.pointerDrag.GetComponent<DraggableEntry>();
            if (draggableEntry && !draggableEntry.isUsed && _textMesh)
            {
                _notebookEntry = NoteBook.instance.GetEntry(draggableEntry.text);

                if (_notebookEntry != null)
                {
                    _textMesh.text = _notebookEntry.associatedText;
                    draggableEntry.isUsed = true;
                }

                if (_journal && _multiplier && _notebookEntry != null)
                {
                    _journal.AddAppreciation(Factions.Government, _notebookEntry.government, _multiplier.multiplier);
                    _journal.AddAppreciation(Factions.Rebels, _notebookEntry.rebels, _multiplier.multiplier);
                    _journal.AddAppreciation(Factions.People, _notebookEntry.people, _multiplier.multiplier);
                }
            }
        }
    }
    
}
