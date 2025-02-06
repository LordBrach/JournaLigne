using System;
using TMPro;
using UnityEngine;

public class NoteBookUI : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform contentParent;
    
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI popupText;

    private void Awake()
    {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    private void Start()
    {
        DisplayNotebook();
        NoteBook.instance.NewEntryAdded += OnNewEntryAdded;
    }

    private void OnDisable()
    {
        NoteBook.instance.NewEntryAdded -= OnNewEntryAdded;
    }

    private void OnNewEntryAdded(int number)
    {
        popupText.enabled = true;
        popupText.text = "+" + number;
        popupText.gameObject.GetComponent<Animator>().SetTrigger("Popup");
        
        DisplayNotebook();
    }

    public void DisplayNotebook()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        var entries = NoteBook.instance.GetEntries();
        foreach (var entry in entries)
        {
            var newEntry = Instantiate(entryPrefab, contentParent);
            var text = newEntry.GetComponentInChildren<TextMeshProUGUI>();

            text.text = "- " + entry.dialoguePhrase;
            
            DraggableEntry dragEntry = newEntry.GetComponent<DraggableEntry>();
            if (dragEntry)
            {
                dragEntry.text = entry.dialoguePhrase;
            }
        }
    }

    public void ShowNotebook(bool show)
    {
        if (!animator) return;
        
        animator.SetBool("OpenClose", show);
    }

    public void ShowNotebookInGame(bool show)
    {
        if (!animator) return;

         animator.SetBool("OpenCloseInGame", show);
         
         if (show)
             DisplayNotebook();
    }
    
}
