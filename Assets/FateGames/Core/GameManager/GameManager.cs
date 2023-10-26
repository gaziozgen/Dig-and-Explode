using DG.Tweening;
using GameAnalyticsSDK;
using Lofelt.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FateGames.Core
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Properties")]
        [SerializeField] private GameStateVariable gameState;
        [SerializeField] private SplashScreen splashScreen;
        [Header("Target Frame Rate")]
        [SerializeField] private int defaultTargetFrameRate = -1;
        [Header("Save Management")]
        [SerializeField] private bool autoSave = false;
        [SerializeField] private float autoSavePeriod = 10;
        [Header("Scene Management")]
        [SerializeField] private int firstLevelSceneIndex = 1;
        [SerializeField] private bool loop;
        [SerializeField] private GameObject loadingScreen;
        [Header("Level Management")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private GameObject loseScreen, winScreen;
        [Header("Sound Management")]
        [SerializeField] private BoolVariable soundOn;
        [SerializeField] private GameObject soundWorkerPrefab;
        [SerializeField] private WorkingSoundWorkerSet workingWorkerSet;
        [SerializeField] private AvailableSoundWorkerSet availableWorkerSet;
        [Header("Haptic Management")]
        [SerializeField] private BoolVariable vibrationOn;
        [Header("Firebase")]
        [SerializeField] private FirebaseManager firebaseManager;
        [SerializeField] private RemoteConfigManager remoteConfigManager;
        private PushNotificationManager pushNotificationManager = new();
        [Header("Applovin")]
        [SerializeField] private ApplovinManager applovinManager;
        [Header("Adjust")]
        [SerializeField] private AdjustManager adjustManager;
        [Header("Events")]
        [SerializeField] private UnityEvent onPause;
        [SerializeField] private UnityEvent onResume, onLevelStarted, onLevelWon, onLevelFailed, onLevelCompleted;
        private GamePauser gamePauser;
        private SceneManager sceneManager;
        private LevelManager levelManager;
        private SoundManager soundManager;
        private HapticManager hapticManager;
        private FacebookManager facebookManager = new();
        private bool thirdPartyInitialized = false;

        protected override void Awake()
        {
            base.Awake();
            if (duplicated) return;
            Initialize();
            IEnumerator routine()
            {
                if (splashScreen)
                    yield return new WaitUntil(() => splashScreen.Finished);
                yield return InitializeThirdParty();
                if (!sceneManager.IsLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene()))
                    sceneManager.LoadCurrentLevel();
                else
                {
                    gameState.Value = GameState.BEFORE_START;
                    if (autoStart)
                        StartLevel();
                }
            }
            StartCoroutine(routine());
        }

        private void Start()
        {
            if (autoSave) StartCoroutine(AutoSaveRoutine());

        }

        public void Initialize()
        {
            Debug.Log("Initialize");
            SetTargetFrameRate(defaultTargetFrameRate);
            InitializeGamePauser();
            InitializeSceneManagement();
            InitializeLevelManagement();
            InitializeSoundManagement();
            InitializeHapticManagement();
        }
        public IEnumerator InitializeThirdParty()
        {
            if (firebaseManager)
            {
                yield return firebaseManager.Initialize();
                pushNotificationManager.Initialize();
            }
            if (applovinManager)
                yield return applovinManager.Initialize();
            if (adjustManager)
                adjustManager.Initialize();
            GameAnalytics.Initialize();
            yield return facebookManager.Initialize();
            if (AdManager.Instance)
                AdManager.Instance.Initialize();
            thirdPartyInitialized = true;

        }
        private void InitializeGamePauser()
        {
            gamePauser = new(onPause, onResume, gameState);
        }

        private void InitializeSceneManagement()
        {
            sceneManager = new(gameState, firstLevelSceneIndex, loop, loadingScreen);
        }
        private void InitializeLevelManagement()
        {
            levelManager = new(loseScreen, winScreen, gameState, onLevelStarted, onLevelCompleted, onLevelFailed, onLevelWon);
        }
        private void InitializeSoundManagement()
        {
            soundManager = new(gameState, soundOn, soundWorkerPrefab, workingWorkerSet, availableWorkerSet);
        }
        private void InitializeHapticManagement()
        {
            hapticManager = new(vibrationOn);
        }

        public long GetNumberConfig(NumberRemoteConfig numberRemoteConfig)
        {
            return remoteConfigManager.GetNumberConfig(numberRemoteConfig);
        }

        public void StartLevel()
        {
            levelManager.StartLevel();
            ReportLevelProgress(GAProgressionStatus.Start);
        }

        public void FinishLevel(bool success)
        {
            ReportLevelProgress(success ? GAProgressionStatus.Complete : GAProgressionStatus.Fail);
            levelManager.FinishLevel(success);
        }

        public void LoadCurrentLevel()
        {
            soundManager.StopWorkers();
            sceneManager.LoadCurrentLevel();
        }

        private IEnumerator AutoSaveRoutine()
        {
            yield return new WaitForSeconds(autoSavePeriod);
            SaveManager.Instance.Save();
            yield return AutoSaveRoutine();
        }

        public void SetTargetFrameRate(int targetFrameRate) => Application.targetFrameRate = targetFrameRate;

        public void PauseGame()
        {
            gamePauser.PauseGame();
        }
        public void ResumeGame()
        {
            gamePauser.ResumeGame();
        }

        public void PlaySoundOneShot(SoundEntity entity)
        {
            PlaySound(entity);
        }

        public SoundWorker PlaySound(SoundEntity entity, bool ignoreListenerPause = false)
        {
            return PlaySound(entity, Vector3.zero, ignoreListenerPause);
        }

        public SoundWorker PlaySound(SoundEntity entity, Vector3 position, bool ignoreListenerPause = false, bool pauseOnStartIfGamePaused = false)
        {
            return soundManager.PlaySound(entity, position, ignoreListenerPause, pauseOnStartIfGamePaused);
        }

        public void PlayHaptic()
        {
            hapticManager.PlayHaptic();
        }
        public void PlayHaptic(HapticPatterns.PresetType presetType)
        {
            hapticManager.PlayHaptic(presetType);
        }

        void OnEnable()
        {
            //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnLevelFinishedUnloading;
        }

        void OnDisable()
        {
            //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnLevelFinishedUnloading;
        }

        public void ReportLevelProgress(GAProgressionStatus status)
        {
            GameAnalytics.NewProgressionEvent(status, "Level_Progress", SaveManager.Level);
        }

        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (thirdPartyInitialized && sceneManager.IsLevel(scene))
            {
                gameState.Value = GameState.BEFORE_START;
                if (autoStart)
                    StartLevel();
            }
        }
        void OnLevelFinishedUnloading(Scene scene)
        {
            Debug.Log($"Killed {DOTween.KillAll()} tweens!");
            SaveManager.Instance.Save();
        }

        public void SetTimeScale(float timeScale)
        {
            if(gameState.Value == GameState.PAUSED)
            {
                Debug.LogError("Game is paused!");
                return;
            }
            Time.timeScale = timeScale;
        }

    }
}
