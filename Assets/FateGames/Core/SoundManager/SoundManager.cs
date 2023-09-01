using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames.Core
{
    public class SoundManager
    {
        private GameStateVariable gameState;
        private BoolVariable soundOn;
        private GameObject soundWorkerPrefab;
        private WorkingSoundWorkerSet workingWorkerSet;
        private AvailableSoundWorkerSet availableWorkerSet;

        public SoundManager(GameStateVariable gameState, BoolVariable soundOn, GameObject soundWorkerPrefab, WorkingSoundWorkerSet workingWorkerSet, AvailableSoundWorkerSet availableWorkerSet)
        {
            this.gameState = gameState;
            this.soundOn = soundOn;
            this.soundWorkerPrefab = soundWorkerPrefab;
            this.workingWorkerSet = workingWorkerSet;
            this.availableWorkerSet = availableWorkerSet;
            Initialize();
        }

        private int workerCount { get => workingWorkerSet.Count + availableWorkerSet.Count; }

        public void Initialize()
        {
            soundOn.Value = true;
        }

        public void StopWorkers()
        {
            for (int i = 0; i < workingWorkerSet.Count; i++)
            {
                SoundWorker worker = workingWorkerSet[i];
                worker.Stop();
            }
        }

        private SoundWorker GetAvailableWorker()
        {
            SoundWorker worker = null;
            void GetWorker() { if (availableWorkerSet.Count > 0) worker = availableWorkerSet[0]; }
            GetWorker();
            if (worker == null)
            {
                DoubleWorkers();
                GetWorker();
            }
            return worker;
        }

        private void DoubleWorkers()
        {
            int number = workerCount > 0 ? workerCount : 1;
            for (int i = 0; i < number; i++)
                InstantiateWorker();
        }

        private void InstantiateWorker()
        {
            Object.Instantiate(soundWorkerPrefab);
        }

        public SoundWorker PlaySound(SoundEntity entity, bool ignoreListenerPause = false)
        {
            return PlaySound(entity, Vector3.zero, ignoreListenerPause);
        }

        public SoundWorker PlaySound(SoundEntity entity, Vector3 position, bool ignoreListenerPause = false, bool pauseOnStartIfGamePaused = false)
        {
            if (!entity) return null;
            if (!ignoreListenerPause && !pauseOnStartIfGamePaused && gameState.Value == GameState.PAUSED) return null;
            SoundWorker worker = GetAvailableWorker();
            float pitch = Random.Range(entity.PitchRangeMin, entity.PitchRangeMax);
            worker.Initialize(entity.Clip, entity.Volume, pitch, entity.SpatialBlend, entity.Loop, position, ignoreListenerPause);
            worker.Play();
            if (!ignoreListenerPause && gameState.Value == GameState.PAUSED && pauseOnStartIfGamePaused)
            {
                worker.Pause();
            }
            return worker;
        }

    }
}
