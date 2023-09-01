using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FateGames.Core
{
    public abstract class RuntimeSet<T> : ScriptableObject
    {
        protected List<T> Items = new();
        public event System.Action OnAdd = null;
        public event System.Action OnRemove = null;
        public int Count => Items.Count;
        public void Add(T t)
        {
            if (!Items.Contains(t))
            {
                Items.Add(t);
                OnAdd?.Invoke();
            }
        }

        public void Remove(T t)
        {
            if (Items.Contains(t))
            {
                Items.Remove(t);
                OnRemove?.Invoke();
            }
        }

        public T this[int index]
        {
            get
            {
                return Items[index];
            }
            set
            {
                Items[index] = value;
            }
        }
    }
}
