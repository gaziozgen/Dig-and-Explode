using FateGames.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemPile<T> where T : FateMonoBehaviour
{
    Transform pileCenter;
    Vector3 itemSize;
    Vector2 dimensions;

    List<T> items = new List<T>();
    int floorCapacity;
    Vector3 correction;
    bool defaultDirection;

    public int Count => items.Count;

    public ItemPile(Transform pileCenter, Vector3 itemSize, Vector2 dimensions, bool defaultDirection = true)
    {
        this.pileCenter = pileCenter;
        this.itemSize = itemSize;
        this.dimensions = dimensions;
        this.defaultDirection = defaultDirection;

        floorCapacity = (int)(dimensions.x * dimensions.y);
        correction = -(dimensions.x - 1) * itemSize.x / 2 * (defaultDirection? -pileCenter.right: pileCenter.right);
        correction += -(dimensions.y - 1) * itemSize.z / 2 * pileCenter.forward;
    }

    public void AddItem(T item)
    {
        
        int itemsOnFloor = items.Count % floorCapacity;

        Vector3 pos = items.Count / floorCapacity * itemSize.y * pileCenter.up;
        pos += itemsOnFloor / (int)dimensions.x * itemSize.z * pileCenter.forward;
        pos += itemsOnFloor % (int)dimensions.x * itemSize.x * (defaultDirection ? -pileCenter.right : pileCenter.right);

        item.transform.position = pos + pileCenter.position + correction;
        item.transform.forward = pileCenter.forward;
        items.Add(item);
    }

    public T GetItem()
    {
        T item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        return item;
    }

}
