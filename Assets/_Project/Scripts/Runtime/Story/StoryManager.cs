using System;
using _Project.Scripts.Runtime.Core;
using _Project.Scripts.Runtime.TransitionSystem;
using LittleDialogue.Runtime;
using LittleGraph.Runtime;
using UnityEngine;

namespace _Project.Scripts.Runtime.Story
{
    public class StoryManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private DialogueController m_dialogueController;
        [SerializeField] private LGGraphObject m_dayGraphObject;
        [SerializeField] private ResultGraphSingle m_resultGraph;
        [SerializeField] private Consequences m_consequences;

        [Header("EndGame")]
        [SerializeField] private string m_mainMenuSceneName;
        
        #region Singleton
        private static StoryManager m_instance;
        public static StoryManager Instance => m_instance;
        private void InitSingleton()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                m_instance = this;
            } 
        }
        #endregion
        
        private void Awake()
        {
            InitSingleton();
        }

        private void Start()
        {
            //Initialization of systems//
            //Init Day Manager
            DayManager.instance.Initialize();
            DayManager.instance.OnDayStarted += OnDayStartedEvent;
            DayManager.instance.OnDayChanged += OnDayChangedEvent;
            DayManager.instance.OnDayEnded += OnDayEndedEvent;
            
            //Init Notebook
            NoteBook.instance.Initialize();
            NoteBook.instance.OnNewsPaperValidate += OnNewsPaperValidateEvent;
            
            //Init ResultGraph
            m_resultGraph.Init();
            m_resultGraph.OnButtonClickedDispatcher += OnResultGraphValidatedEvent;
            
            //InitDialogueController
            if (m_dialogueController)
            {
                m_dialogueController.Init();
            }
            
            //Init Consequences
            if (m_consequences)
            {
                m_consequences.Initialize();
                m_consequences.OnConsequencesValidate += OnConsequenceValidateEvent;
            }
            
            
            //Start first day
            DayManager.instance.StartNewDay();
            
        }

        private void OnDayStartedEvent(Days dayStarted)
        {
            switch (dayStarted.dayType)
            {
                case DayType.Interview:
                    m_dayGraphObject.ReplaceGraph(dayStarted.currentGraph);
            
                    m_dialogueController.SubscribeToGraphInstance(m_dayGraphObject.GraphInstance);
            
                    m_dialogueController.ShowDialogue();
                    
                    m_dayGraphObject.ExecuteAsset();
                    break;
                case DayType.Article:
                    NoteBook.instance.ShowNewsPaper();
                    break;
                case DayType.Review:
                    m_consequences.ShowConsequences(dayStarted);
                    break;
                case DayType.EndGame:
                    EndGame();
                    
                    break;
                default:
                    break;
            }
        }

        private void EndGame()
        {
            NoteBook.instance.HideNewsPaper();
            m_consequences.HideConsequences();
            m_resultGraph.Hide();
            m_dialogueController.HideDialogue();
            
            if (GameManager.Instance)
            {
                    GameManager.Instance.LoadScene(m_mainMenuSceneName);    
            }
            else
            {
                Debug.LogError("End Game : No Game Manager Instance");
            }
        }

        private void OnDayChangedEvent(Days newDay)
        {
            
        }
        
        private void OnDayEndedEvent(Days dayEnded)
        {
            switch (dayEnded.dayType)
            {
                case DayType.Interview:
                    m_dialogueController.HideDialogue();
                    break;
                case DayType.Article:
                    NoteBook.instance.RemoveEntries();
                    // NoteBook.instance.HideNewsPaper();
                    m_resultGraph.Hide();
                    break;
                case DayType.Review:
                    m_consequences.HideConsequences();
                    break;
                case DayType.EndGame:
                    break;
                default:
                    break;
            }
        }
        
        private void OnNewsPaperValidateEvent(Appreciations appreciations)
        {
            // DayManager.instance.NextDay();
            m_resultGraph.Show();
            Debug.Log("Show Graph");
        }

        private void OnResultGraphValidatedEvent()
        {
            DayManager.instance.NextDay();
        }
        
        private void OnConsequenceValidateEvent(Days day)
        {
            DayManager.instance.NextDay();
        }
    }
}
