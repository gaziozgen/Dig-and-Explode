using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersonQueue<T> where T : Person
{
    private Func<T, Coroutine> OnReach;
    private Action<Coroutine> stopCoroutine;
    private int maxLength;
    private Transform startPosition = null;
    private float distance;
    private readonly List<T> list = new();

    private Coroutine lastCoroutine = null;

    public int Length => list.Count;

    public PersonQueue(Transform startPosition, float distance, int maxLength, Func<T, Coroutine> onReach, Action<Coroutine> stopCoroutine)
    {
        if (maxLength != -1 && maxLength < 1) Debug.LogError("MaxLength must be positive int if not \"-1\"");
        this.startPosition = startPosition;
        this.distance = distance;
        this.stopCoroutine = stopCoroutine;
        this.maxLength = maxLength - 1;
        OnReach = onReach;
    }

    public int QueueLength() { return list.Count; }

    public void Enqueue(T person)
    {
        list.Add(person);
        AdjustPositionsFrom(list.Count - 1);
    }

    public T Dequeue()
    {
        if (list.Count > 0)
        {
            T person = list[0];
            list.RemoveAt(0);
            //stopCoroutine(lastCoroutine);
            AdjustPositionsFrom(0);
            return person;
        }
        else
        {
            Debug.LogError("Queue Is Empty");
            return null;
        }
    }

    public void Clear()
    {
        list.Clear();
    }

    /*public bool Contains(T person)
    {
        return list.Contains(person);
    }
    
    public void RemoveImmediate(T person)
    {
        if (list.Contains(person))
        {
            int index = list.IndexOf(person);
            list.Remove(person);
            if (index == 0) stopCoroutine(lastCoroutine);
            AdjustPositionsFrom(index);
        }
        else
            Debug.LogError("person not in queue", person);
    }*/

    public T Peek()
    {
        return list[0];
    }

    public void SetStartPosition(Transform newStart)
    {
        startPosition = newStart;
        AdjustPositionsFrom(1);
    }

    private void AdjustPositionsFrom(int updateFromIndex)
    {
        for (int i = updateFromIndex; i < list.Count; i++)
        {
            int order = i;
            if (maxLength != -2 && i > maxLength) order = maxLength;
            list[i].Agent.GoToClosestPoint(startPosition.position - order * distance * startPosition.forward);

            if (i == 0)
            {
                lastCoroutine = OnReach(list[i]);
                if (list[i].Coroutine != null) Debug.LogError("not empty coroutine", list[i]);
                list[i].Coroutine = lastCoroutine;
            }
        }
    }
}