using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public enum Levels
        {
            Menu = -1,
            Level1 = 0,
        }

        [SerializeField] private LoadingScreen loadingScreen;

        [SerializeField] private string menu;

        [SerializeField] private List<string> levels;

        public void SwitchToLevel(Levels levelName)
        {
            string nextLevelName;
            switch (levelName)
            {
                case Levels.Menu:
                    nextLevelName = menu;
                    break;
                default:
                    nextLevelName = levels[(int) levelName];
                    break;
            }

            StartCoroutine(UnLoadLevel(SceneManager.GetActiveScene().name, LoadLevel(nextLevelName)));
        }

        private void Awake()
        {
            GameManager.Instance = this;
        }

        private void Start()
        {
            StartCoroutine(LoadLevel(menu));
        }

        private IEnumerator LoadLevel(string sceneName)
        {
            loadingScreen.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                loadingScreen.Progress = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f) asyncOperation.allowSceneActivation = true;
                yield return null;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
            loadingScreen.gameObject.SetActive(false);
        }

        private IEnumerator LoadLevelNoLoadingScreen(string sceneName)
        {
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!asyncOperation.isDone) yield return null;
            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        private IEnumerator UnLoadLevel(string unloadSceneName, IEnumerator loadScene)
        {
            loadingScreen.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();

            var asyncOperation = SceneManager.UnloadSceneAsync(unloadSceneName);
            while (!asyncOperation.isDone) yield return null;
            StartCoroutine(loadScene);
        }

        private IEnumerator UnLoadLevel(string unloadSceneName)
        {
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.UnloadSceneAsync(unloadSceneName);
            while (!asyncOperation.isDone) yield return null;
        }
    }
}