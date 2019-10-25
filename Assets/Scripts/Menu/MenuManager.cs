using Game;

namespace Menu
{
    public class MenuManager : UnityEngine.MonoBehaviour
    {
        public void StartGame()
        {
            GameManager.Instance.SwitchToLevel(GameManager.Levels.Level1);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Options()
        {
        }
    }
}