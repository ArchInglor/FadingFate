using System.Collections.Generic;
using UnityEngine;

public class PoolMono<T> where T : MonoBehaviour
{
    public int Count => _pool.Count;

    private T _template;
    private bool _autoExpand;
    private Transform _container;

    private List<T> _pool;

    public PoolMono(T template, int count, bool autoExpand, Transform container = null)
    {
        _template = template;
        _autoExpand = autoExpand;
        _container = container;

        CreetePool(count);
    }

    private void CreetePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(_template, _container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        _pool.Add(createdObject);
        return createdObject;
    }

    public bool TryGetFreeElement(out T element, bool SetActive = true)
    {
        foreach (var mono in _pool)
        {
            if (!mono.gameObject.activeInHierarchy)
            {
                element = mono;
                element.gameObject.SetActive(SetActive);
                return true;
            }
        }

        element = null;
        return false;
    }

    public T GetFreeElement(bool SetActive = true)
    {
        if (TryGetFreeElement(out var element, SetActive))
        {
            return element;
        }

        if (_autoExpand)
        {
            return CreateObject(true);
        }

        throw new System.Exception($"There is no free elements of type {typeof(T)}");
    }
}
