using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class AudioComponent : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioMixerGroup m_audioMixerGroup;

        [SerializeField] private AudioClip[] m_audioClip;

        public void PlayOneShotSound2D()
        {
            if (AudioManager.Instance)
            {
                if(m_audioClip.Length == 1)
                {
                    AudioManager.Instance.PlayOneShotSound2D(m_audioClip[0], m_audioMixerGroup);
                }
                if (m_audioClip.Length > 1)
                {
                    int i = Random.Range(0, m_audioClip.Length);
                    AudioManager.Instance.PlayOneShotSound2D(m_audioClip[i], m_audioMixerGroup);
                }
            }
        }

        public void PlayMusic()
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlayMusic(m_audioClip[0]);
            }
        }

        public void StopMusic()
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.StopMusic();
            }
        }
    }
}
