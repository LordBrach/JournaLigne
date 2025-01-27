using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LittleDialogue.Runtime
{
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] private GameObject m_dialogueBoxPanel;
        [SerializeField] private TextMeshProUGUI m_dialogueText;
        [SerializeField] private GameObject m_choiceButtonsParent;
        [SerializeField] private GameObject m_choiceButtonPrefab;
        [SerializeField] private List<Button> m_choiceButtons;

        // public GameObject DialogueBoxPanel => m_dialogueBoxPanel;
        // public TextMeshProUGUI DialogueText => m_dialogueText;
        // public List<Button> ChoiceButtons => m_choiceButtons;

        private void OnEnable()
        {
            
        }

        public void ShowBox()
        {
            m_dialogueBoxPanel.gameObject.SetActive(true);
        }

        public void UpdateText(string newText)
        {
            m_dialogueText.text = newText;
        }

        public void ClearChoiceButtons()
        {
            for (int i = m_choiceButtons.Count - 1; i >= 0; i--)
            {
                Button button = m_choiceButtons[i];
                m_choiceButtons.RemoveAt(i);
                Destroy(button.gameObject);
            }
        }
        
        public void AddChoiceButton(string buttonText = "Null", UnityAction callback = null)
        {
            Button button = Instantiate(m_choiceButtonPrefab, m_choiceButtonsParent.transform).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
            button.onClick.AddListener(callback);
            m_choiceButtons.Add(button);
        }

        // public void UpdateChoiceButtonTexts(params string[] options)
        // {
        //     for (int i = 0; i < options.Length; i++)
        //     {
        //         if(i>m_choiceButtons.Count-1) break;
        //         m_choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i];
        //     }
        // }
    }
}
