using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Queue<T> objects = new Queue<T>();

    public ObjectPool(T prefab, int initialSize, Transform pool)
    {
        this.prefab = prefab;
        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(prefab, pool);
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (objects.Count == 0)
        {
            T obj = GameObject.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            objects.Enqueue(obj);
        }
        T pooledObj = objects.Dequeue();
        pooledObj.gameObject.SetActive(true);
        return pooledObj;
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        objects.Enqueue(obj);
    }
}
