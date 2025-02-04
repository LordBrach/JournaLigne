using UnityEngine;

namespace _Project.Scripts.Runtime.Core
{
    public class ChangeSceneComponent : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.LoadScene(sceneName);
            }
        }
    }
}
