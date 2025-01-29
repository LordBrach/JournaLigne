using UnityEngine;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton

        private static AudioManager m_instance;
        public static AudioManager Instance => m_instance;

        private void InitSingleton()
        {
            if (m_instance != this && m_instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        #endregion
    }
}
