using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class AudioComponent : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioMixerGroup m_audioMixerGroup;

        [SerializeField] private AudioClip m_audioClip;

        public void PlayOneShotSound2D()
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlayOneShotSound2D(m_audioClip, m_audioMixerGroup);
            }
        }
    }
}
