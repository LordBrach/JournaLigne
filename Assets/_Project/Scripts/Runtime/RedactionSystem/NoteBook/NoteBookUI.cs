using System;
using TMPro;
using UnityEngine;

public class NoteBookUI : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform contentParent;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
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
        _animator.SetBool("OpenClose", show);
    }
    
}
