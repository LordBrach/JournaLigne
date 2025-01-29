using System;
using LittleDialogue.Runtime;
using LittleGraph.Runtime;
using UnityEngine;

namespace _Project.Scripts.Runtime.Story
{
    public class StoryManager : MonoBehaviour
    {
        [SerializeField] private DialogueController m_dialogueController;
        [SerializeField] private LGGraphObject m_dayGraphObject;
        
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

            
            //InitDialogueController
            if (m_dialogueController)
            {
                m_dialogueController.Init();
            }
            
            
            //Start first day
            DayManager.instance.StartNewDay();
            
            //Start Dialogue of new day
        }

        private void OnDayStartedEvent(Days dayStarted)
        {
            switch (dayStarted.dayType)
            {
                case DayType.Interview:
                    m_dayGraphObject.ReplaceGraph(dayStarted.currentGraph);
            
                    m_dialogueController.SubscribeToGraphInstance(m_dayGraphObject.GraphInstance);
            
                    m_dayGraphObject.ExecuteAsset();
                    break;
                case DayType.Article:
                    break;
                case DayType.Review:
                    break;
                case DayType.EndGame:
                    break;
                default:
                    break;
            }
        }
        
        private void OnDayChangedEvent(Days newDay)
        {
            throw new NotImplementedException();
        }
        
        private void OnDayEndedEvent(Days dayEnded)
        {
            throw new NotImplementedException();
        }
    }
}
