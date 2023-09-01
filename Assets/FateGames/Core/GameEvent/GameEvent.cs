using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames.Core
{
    [CreateAssetMenu(menuName = "Fate/Event", fileName = "Game Event")]
    public class GameEvent : ScriptableObject
    {
#if UNITY_EDITOR
        public List<GameObject> listenerObjects = new();
#endif
        [SerializeField] bool logRaise = false;
        private List<GameEventListener> listeners = new List<GameEventListener>();
        public event System.Action OnRaise;
        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
                listeners[i].OnEventRaised();
            OnRaise?.Invoke();
#if UNITY_EDITOR
            if (logRaise)
            {
                Debug.Log($"Event {name} raised!", this);
            }
#endif
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
#if UNITY_EDITOR
            listenerObjects.Add(listener.gameObject);
#endif
        }
        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
#if UNITY_EDITOR
            listenerObjects.Remove(listener.gameObject);
#endif
        }
    }
}