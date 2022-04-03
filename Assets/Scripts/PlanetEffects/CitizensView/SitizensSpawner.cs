using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RandomPointPicker))]
public class SitizensSpawner : MonoBehaviour
{
    [SerializeField] private Sitizen _sitizenTemplate;
    [SerializeField] private int _maxSitizensCount;
    private PoolMono<Sitizen> _sitizenPool;

    private RandomPointPicker _randomPointPicker;

    private void Awake()
    {
        _randomPointPicker = GetComponent<RandomPointPicker>();
        _sitizenPool = new PoolMono<Sitizen>(_sitizenTemplate, _maxSitizensCount, false, transform);
    }

    private void Start()
    {
        SpawnCitizens();
    }

    public void SpawnCitizens()
    {
        for (int i = 0; i < _sitizenPool.Count; i++)
        {
            var sitizen = _sitizenPool.GetFreeElement();
            sitizen.transform.localPosition = _randomPointPicker.GetRandomPointInCollider();
        }
    }
}
