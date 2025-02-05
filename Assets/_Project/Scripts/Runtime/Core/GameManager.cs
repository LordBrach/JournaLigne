using System;
using _Project.Scripts.Runtime.Core.AudioSystem;
using _Project.Scripts.Runtime.TransitionSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Runtime.Core
{
    public class GameManager : MonoBehaviour
    {
        private string m_sceneNameToBeLoaded;
        
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
            m_sceneNameToBeLoaded = sceneName;

            if (TransitionManager.Instance)
            {
                TransitionManager.Instance.OnFadeInEnded += OnLoadSceneFadeInEnded;
                TransitionManager.Instance.FadeIn();
            }
            // SceneManager.LoadScene(sceneName);
        }

        private void OnLoadSceneFadeInEnded()
        {
            TransitionManager.Instance.OnFadeInEnded -= OnLoadSceneFadeInEnded;
            SceneManager.LoadScene(m_sceneNameToBeLoaded);
            TransitionManager.Instance.FadeOut();
        }
    }
}
