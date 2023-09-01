using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace FateGames.Core
{
    public class SceneManager
    {
        private GameStateVariable gameState;
        private int firstLevelSceneIndex;
        private bool loop;
        private GameObject loadingScreenPrefab;


        public SceneManager(GameStateVariable gameState, int firstLevelSceneIndex, bool loop, GameObject loadingScreenPrefab)
        {
            this.gameState = gameState;
            this.firstLevelSceneIndex = firstLevelSceneIndex;
            this.loop = loop;
            this.loadingScreenPrefab = loadingScreenPrefab;
        }

        private int levelCount { get => UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - firstLevelSceneIndex; }
        public bool IsLevel(UnityEngine.SceneManagement.Scene scene) => scene.buildIndex >= firstLevelSceneIndex;
        private int currentLevelSceneIndex
        {
            get
            {
                if (loop)
                {
                    int loopedLevel = SaveManager.Level % levelCount;
                    if (loopedLevel == 0) loopedLevel = levelCount;
                    return firstLevelSceneIndex - 1 + loopedLevel;
                }
                return SaveManager.Level;
            }
        }

        public void LoadCurrentLevel(bool async = true)
        {
            LoadScene(currentLevelSceneIndex, async);
        }

        public void LoadScene(int sceneIndex, bool async = true)
        {
            if (sceneIndex < 0 || sceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                throw new System.ArgumentOutOfRangeException();
            gameState.Value = GameState.LOADING;
            if (async)
                GameManager.Instance.StartCoroutine (LoadSceneAsynchronouslyRoutine(sceneIndex));
            else UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        private IEnumerator LoadSceneAsynchronouslyRoutine(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
                throw new System.ArgumentOutOfRangeException();
            Object.Instantiate(loadingScreenPrefab);
            AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                yield return null;
            }
            if (operation.isDone)
            {

            }
        }

#if UNITY_EDITOR
        [MenuItem("Fate/Scene/Open Loading Screen")]
        public static void OpenLoadingScreen()
        {
            AssetDatabase.OpenAsset(Resources.Load("Screens/LoadingScreen"));
        }
#endif
    }

}
