using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common
{
    public sealed class Actions : MonoBehaviour
    {
        public void OnQuit()
        {
            SceneManager.LoadScene(Scenes.MainMenu, LoadSceneMode.Single);
        }
    }
}