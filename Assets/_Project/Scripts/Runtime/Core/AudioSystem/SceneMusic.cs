using UnityEngine;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip m_musicAudioClip;
        
        // Start is called before the first frame update
        void Start()
        {
            AudioManager.Instance?.PlayMusic(m_musicAudioClip);
        }

        public void StopMusic()
        {
            AudioManager.Instance?.StopMusic();
        }
    }
}
