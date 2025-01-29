using System;
using UnityEngine;

namespace _Project.Scripts.Runtime.Story
{
    public class StoryManager : MonoBehaviour
    {
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
            //Initialization of systems
            DayManager.instance.Initialize();
            //...
            
            
            //Start first day
            
        }
    }
}
