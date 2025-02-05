using System;
using _Project.Scripts.Runtime.Core.AudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.Core
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager m_instance;
        public static GameManager Instance => m_instance;

        private void InitSingleton()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                m_instance = this;
                DontDestroyOnLoad(gameObject);
            } 
        }
        #endregion
        
        private void Awake()
        {
            InitSingleton();
        }

        //Scene must be in build settings
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
