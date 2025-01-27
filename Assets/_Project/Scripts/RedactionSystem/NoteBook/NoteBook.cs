using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NotebookEntry
{
    public string dialoguePhrase;
    public string associatedText;
    public float rebels;          // Rebles
    public float people;          // people
    public float government;     // government
}

public class NoteBook : MonoBehaviour
{
    public static NoteBook instance;

    [SerializeField] private List<NotebookEntry> notebookEntries = new List<NotebookEntry>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// New entry for notebook.
    /// </summary>
    /// <param name="dialoguePhrase">The sentence from the dialogues.</param>
    /// <param name="associatedText">Title of news.</param>
    /// <param name="rebels">rebels appreciation.</param>
    /// <param name="people">people appreciation.</param>
    /// <param name="government">government appreciation.</param>
    public void AddEntry(string dialoguePhrase, string associatedText, float rebels, float people, float government)
    {
        NotebookEntry newEntry = new NotebookEntry
        {
            dialoguePhrase = dialoguePhrase,
            associatedText = associatedText,
            rebels = rebels,
            people = people,
            government = government
        };

        notebookEntries.Add(newEntry);
        Debug.Log($"Nouvelle entrée ajoutée : {dialoguePhrase}");
    }

    /// <summary>
    /// Get all entry of notebook.
    /// </summary>
    /// <returns>Entry List.</returns>
    public List<NotebookEntry> GetEntries()
    {
        return notebookEntries;
    }

    /// <summary>
    /// Show Entry in console (debug).
    /// </summary>
    public void PrintEntries()
    {
        foreach (var entry in notebookEntries)
        {
            Debug.Log($"Phrase : {entry.dialoguePhrase} | Texte : {entry.associatedText} | Values: {entry.rebels}, {entry.people}, {entry.government}");
        }
    }
}
