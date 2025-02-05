using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.TransitionSystem
{
    public class TransitionManager : MonoBehaviour
    {
        //Fade fields
        [Header("Fade Elements")]
        [SerializeField] private Image m_transitionPanelImage;

        [Header("Fade Parameters")] 
        [SerializeField, Tooltip("In seconds")] private float m_fadeInSpeed;
        [SerializeField, Tooltip("In seconds")] private float m_fadeOutSpeed;
        
        //Fade Coroutines
        private Coroutine m_fadeInCoroutine;
        private Coroutine m_fadeOutCoroutine;
        
        //Fade events
        public event Action OnFadeInStarted;
        public event Action OnFadeInEnded;
        
        public event Action OnFadeOutStarted;
        public event Action OnFadeOutEnded;
        
        #region Singleton
        private static TransitionManager _instance;
        public static TransitionManager Instance => _instance;
        
        private void InitSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }
        #endregion

        private void Awake()
        {
            InitSingleton();
        }

        #region FadeIn
        public void FadeIn()
        {
            StopFadeInCoroutine();
            StopFadeOutCoroutine();
            
            m_fadeInCoroutine = StartCoroutine(FadeInCoroutine());
        }

        private IEnumerator FadeInCoroutine()
        {
            m_transitionPanelImage.raycastTarget = true;
            OnFadeInStarted?.Invoke();
            
            if (m_fadeInSpeed != 0.0f)
            {
                while (m_transitionPanelImage.color.a < 1.0f)
                {
                    float delta = (Time.deltaTime * 1.0f) / m_fadeInSpeed;
                
                    Color tColor = m_transitionPanelImage.color;
                    tColor.a = Mathf.Min(1.0f, tColor.a + delta);
                    m_transitionPanelImage.color = tColor;
                
                    yield return null;
                }
            }
            else
            {
                Color tColor = m_transitionPanelImage.color;
                tColor.a = 1.0f;
                m_transitionPanelImage.color = tColor;
            }
            
            OnFadeInEnded?.Invoke();
        }

        private void StopFadeInCoroutine()
        {
            if(m_fadeInCoroutine == null) return;
            
            StopCoroutine(m_fadeInCoroutine);
            m_fadeInCoroutine = null;
        }

        #endregion

        #region FadeOut

        public void FadeOut()
        {
            StopFadeInCoroutine();
            StopFadeOutCoroutine();

            m_fadeOutCoroutine = StartCoroutine(FadeOutCoroutine());
        }
        
        private IEnumerator FadeOutCoroutine()
        {
            OnFadeOutStarted?.Invoke();
            if (m_fadeOutSpeed != 0)
            {
                while (m_transitionPanelImage.color.a > 0.0f)
                {
                    float delta = (Time.deltaTime * 1.0f) / m_fadeInSpeed;
                
                    Color tColor = m_transitionPanelImage.color;
                    tColor.a = Mathf.Max(0.0f, tColor.a - delta);
                    m_transitionPanelImage.color = tColor;
                
                    yield return null;
                }
            }
            else
            {
                Color tColor = m_transitionPanelImage.color;
                tColor.a = 0.0f;
                m_transitionPanelImage.color = tColor;
            }

            m_transitionPanelImage.raycastTarget = false;
            OnFadeOutEnded?.Invoke();
        }

        private void StopFadeOutCoroutine()
        {
            if(m_fadeOutCoroutine == null) return;
            
            StopCoroutine(m_fadeOutCoroutine);
            m_fadeOutCoroutine = null;
        }
        #endregion
        
    }
}
