using System;
using System.Collections;
using System.Collections.Generic;
using LittleDialogue.Runtime.LittleGraphAddOn;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LittleDialogue.Runtime
{
    public enum DialogueBoxState
    {
        None, 
        Idle,
        UpdatingText
    }
    
    public class DialogueBox : MonoBehaviour
    {
        [SerializeField] private GameObject m_dialogueBoxPanel;
        
        [Header("Environment")]
        //Interlocutor
        [SerializeField] private Image m_interlocutorImage;

        [SerializeField] private Color m_whiteColor;
        [SerializeField] private Color m_greyColor;
        //Background
        [SerializeField] private Image m_backgroundImage;
        //ForeGround
        [SerializeField] private Image m_foregroundImage;
        
        [Header("Dialogue & Choices")]
        //Dialogue and Choices
        [SerializeField] private TextMeshProUGUI m_dialogueText;
        [SerializeField] private GameObject m_choiceButtonsParent;
        [SerializeField] private GameObject m_choiceButtonPrefab;
        [SerializeField] private List<Button> m_choiceButtons;

        private DialogueBoxState m_dialogueBoxState = DialogueBoxState.Idle;
        
        // COROUTINES //
        private Coroutine m_updateTextCoroutine;
        
        [Header("Dialogue Text Writing")]
        [SerializeField] private float m_textWritingSpeed;

        private string m_cachedText;
        
        // ACTIONS & EVENTS//
        
        
        //Text Update Events
        public event UnityAction OnCharacterWritten;
        public event UnityAction OnTextUpdateEnded;

        [Header("Text Update Events")] [SerializeField]
        private UnityEvent OnCharacterWrittenEvent;
        [SerializeField] private UnityEvent OnTextUpdateEndedEvent;
        
        //Touch Events
        public event UnityAction OnUpdatingTextBoxTouched; 
        public event UnityAction OnIdleTextBoxTouched;

        [Header("Text Box Touch Events")]
        [SerializeField] private UnityEvent OnTextBoxTouchedEvent;
        
        
        // PROPERTIES //

        #region Properties

        public Image InterlocutorImage => m_interlocutorImage;

        public Image BackgroundImage => m_backgroundImage;

        #endregion
        
        
        // public GameObject DialogueBoxPanel => m_dialogueBoxPanel;
        // public TextMeshProUGUI DialogueText => m_dialogueText;
        // public List<Button> ChoiceButtons => m_choiceButtons;

        private void Awake()
        {
            OnCharacterWrittenEvent.AddListener(() => OnCharacterWritten?.Invoke());
            OnTextUpdateEndedEvent.AddListener(() => OnTextUpdateEnded?.Invoke());
        }

        #region Show/Hide Methods
        public void ShowDialoguePanel()
        {
            m_dialogueBoxPanel.gameObject.SetActive(true);
        }

        public void HideDialoguePanel()
        {
            m_dialogueBoxPanel.gameObject.SetActive(false);
        }

        public void ChangeBackgroundDisplay(LDDialogueDisplayType displayType)
        {
            switch (displayType)
            {
                case LDDialogueDisplayType.Show:
                    ShowBackground();
                    break;
                case LDDialogueDisplayType.Hide:
                    HideBackground();
                    break;
                case LDDialogueDisplayType.None:
                    break;
            }
        }

        private void ShowBackground()
        {
            m_backgroundImage.gameObject.SetActive(true);
        }

        private void HideBackground()
        {
            m_backgroundImage.gameObject.SetActive(false);
        }

        public void ChangeForegroundDisplay(LDDialogueDisplayType displayType)
        {
            switch (displayType)
            {
                case LDDialogueDisplayType.Show:
                    ShowForeground();
                    break;
                case LDDialogueDisplayType.Hide:
                    HideForeground();
                    break;
                case LDDialogueDisplayType.None:
                    break;
            }
        }

        private void ShowForeground()
        {
            m_foregroundImage.gameObject.SetActive(true);
        }

        private void HideForeground()
        {
            m_foregroundImage.gameObject.SetActive(false);
        }

        public void ChangeInterlocutorDisplay(LDDialogueDisplayType displayType)
        {
            switch (displayType)
            {
                case LDDialogueDisplayType.Show:
                    ShowInterlocutor();
                    break;
                case LDDialogueDisplayType.Hide:
                    HideInterlocutor();
                    break;
                case LDDialogueDisplayType.None:
                    break;
            }
        }

        private void ShowInterlocutor()
        {
            m_interlocutorImage.gameObject.SetActive(true);
        }

        private void HideInterlocutor()
        {
            m_interlocutorImage.gameObject.SetActive(false);
        }
        #endregion

        
        #region UpdateMethods
        
        public void UpdateText(string newText)
        {
            //Interrupt Text update
            if(m_updateTextCoroutine != null)
            {
                StopCoroutine(m_updateTextCoroutine);
                m_updateTextCoroutine = null;
            }

            if (m_updateTextCoroutine == null)
            {
                m_updateTextCoroutine = StartCoroutine(UpdateTextCoroutine(newText));
                m_dialogueBoxState = DialogueBoxState.UpdatingText;
            }
        }

        private IEnumerator UpdateTextCoroutine(string newText)
        {
            m_cachedText = newText;
            m_dialogueText.text = "";

            for (int i = 0; i < m_cachedText.Length; i++)
            {
                m_dialogueText.text += m_cachedText[i];

                OnCharacterWrittenEvent.Invoke();
                
                if (m_textWritingSpeed > 0)
                {
                    yield return new WaitForSeconds(1.0f / m_textWritingSpeed);
                }
                else
                {
                    yield return null;
                }
            }

            UpdateTextEnd();
        }

        private void UpdateTextEnd()
        {
            if (m_updateTextCoroutine == null) return;
            
            StopCoroutine(m_updateTextCoroutine);
            m_updateTextCoroutine = null;
            
            m_dialogueBoxState = DialogueBoxState.Idle;
            m_dialogueText.text = m_cachedText;
            
            OnTextUpdateEndedEvent.Invoke();
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


        public void UpdateInterlocutorImage(Sprite inNewSprite, LDInterlocutorColorType interlocutorColorType)
        {
            if(!inNewSprite) return;
            if(!m_interlocutorImage) return;

            m_interlocutorImage.sprite = inNewSprite;
            // Debug.Log(interlocutorColorType);
            switch (interlocutorColorType)
            {
                case LDInterlocutorColorType.White:
                    m_interlocutorImage.color = m_whiteColor;
                    // Debug.Log("White");
                    break;
                case LDInterlocutorColorType.Grey:
                    // Debug.Log("Grey");
                    m_interlocutorImage.color = m_greyColor;
                    break;
                case LDInterlocutorColorType.None:
                    // Debug.Log("None");
                    break;
            }
        }
        
        public void UpdateBackgroundImage(Sprite inNewSprite)
        {
            if(!inNewSprite) return;
            if(!m_backgroundImage) return;

            m_backgroundImage.sprite = inNewSprite;
        }

        public void UpdateForegroundImage(Sprite inNewSprite)
        {
            if(!inNewSprite) return;
            if(!m_foregroundImage) return;

            m_foregroundImage.sprite = inNewSprite;
        }
        
        // public void UpdateChoiceButtonTexts(params string[] options)
        // {
        //     for (int i = 0; i < options.Length; i++)
        //     {
        //         if(i>m_choiceButtons.Count-1) break;
        //         m_choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i];
        //     }
        // }
        #endregion

        #region ReactToInputEvent Methods

        public void OnToucheDialogueBox()
        {
            switch (m_dialogueBoxState)
            {
                case DialogueBoxState.Idle:
                    OnIdleTextBoxTouched?.Invoke();
                    break;
                case DialogueBoxState.UpdatingText:
                    UpdateTextEnd();
                    OnUpdatingTextBoxTouched?.Invoke();
                    break;
                default:
                    break;
            }
            
            OnTextBoxTouchedEvent.Invoke();
        }

        #endregion
    }
}
