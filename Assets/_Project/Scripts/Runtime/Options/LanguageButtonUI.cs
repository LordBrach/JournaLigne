using UnityEngine;

namespace _Project.Scripts.Runtime.Options
{
    public class LanguageButtonUI : MonoBehaviour
    {
        [Header("English")]
        [SerializeField] private Animator m_englishAnimator;
        [SerializeField] private string m_englishSelectAnimName;
        [SerializeField] private string m_englishUnselectAnimName;
        
        [Header("French")]
        [SerializeField] private Animator m_frenchAnimator;
        [SerializeField] private string m_frenchSelectAnimName;
        [SerializeField] private string m_frenchUnselectAnimName;
        
        public void SwitchToFrench()
        {
            m_englishAnimator.Play(m_englishUnselectAnimName);
            m_frenchAnimator.Play(m_frenchSelectAnimName);
        }

        public void SwitchToEnglish()
        {
            m_frenchAnimator.Play(m_frenchUnselectAnimName);
            m_englishAnimator.Play(m_englishSelectAnimName);
        }
    }
}
