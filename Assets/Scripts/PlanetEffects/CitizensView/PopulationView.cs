using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RandomPointPicker))]
public class PopulationView : MonoBehaviour
{
    [SerializeField] private SitizenView _sitizenTemplate;
    [SerializeField] private int _maxSitizenViewsCount;
    [SerializeField] private int _sitizensPerView;
    private ActiveSitizenViewsList _activeSitizens = new ActiveSitizenViewsList();
    private PoolMono<SitizenView> _sitizenPool;

    private Civilization _civilization;
    private RandomPointPicker _randomPointPicker;

    private void Awake()
    {
        _randomPointPicker = GetComponent<RandomPointPicker>();
        _sitizenPool = new PoolMono<SitizenView>(_sitizenTemplate, _maxSitizenViewsCount, false, transform);
        _civilization = GetComponentInParent<Civilization>();
    }

    private void OnEnable()
    {
        _civilization.PopulationChanged += OnPopulationChanged;
    }
    
    private void OnDisable()
    {
        _civilization.PopulationChanged -= OnPopulationChanged;
    }

    private void OnPopulationChanged(Population population)
    {
        UpdateSitizensCount(population.e, SitizenViewType.Economy);
        UpdateSitizensCount(population.r, SitizenViewType.Religion);
        UpdateSitizensCount(population.c, SitizenViewType.Culture);
    }

    public void SpawnSitizens(int count, SitizenViewType type)
    {
        for (int i = 0; i < count; i++)
        {
            var sitizen = _sitizenPool.GetFreeElement();
            sitizen.UpdateType(type);
            sitizen.transform.localPosition = _randomPointPicker.GetRandomPointInCollider();
            _activeSitizens.Add(sitizen);
        }
    }

    public void DeactivateSitizens(int count, SitizenViewType type)
    {
        for (int i = 0; i < count; i++)
        {
            var sitizen = _activeSitizens.GetRandomOfType(type);
            sitizen.gameObject.SetActive(false);
            _activeSitizens.Remove(sitizen);
        }
    }

    public void UpdateSitizensCount(int newSitizensCount, SitizenViewType type)
    {
        var newSitizenViewsCount = Mathf.RoundToInt(newSitizensCount / _sitizensPerView);
        var oldSitizenViewsCount = _activeSitizens.GetCountOfType(type);

        if (newSitizenViewsCount > oldSitizenViewsCount)
        {
            SpawnSitizens(newSitizenViewsCount - oldSitizenViewsCount, type);
        }
        else if (newSitizenViewsCount < oldSitizenViewsCount)
        {
            DeactivateSitizens(newSitizenViewsCount - oldSitizenViewsCount, type);
        }
    }
}


public class ActiveSitizenViewsList
{
    private List<SitizenView> _activeEconomySitizens = new List<SitizenView>();
    private List<SitizenView> _activeReligionSitizens = new List<SitizenView>();
    private List<SitizenView> _activeCultureSitizens = new List<SitizenView>();

    public int GetCountOfType(SitizenViewType type)
    {
        switch (type)
        {
            case SitizenViewType.Economy:
                return _activeEconomySitizens.Count;
            case SitizenViewType.Religion:
                return _activeReligionSitizens.Count;
            case SitizenViewType.Culture:
                return _activeCultureSitizens.Count;
        }

        return 0;
    }

    public void Add(SitizenView sitizen)
    {
        switch (sitizen.Type)
        {
            case SitizenViewType.Economy:
                _activeEconomySitizens.Add(sitizen);
                break;
            case SitizenViewType.Religion:
                _activeReligionSitizens.Add(sitizen);
                break;
            case SitizenViewType.Culture:
                _activeCultureSitizens.Add(sitizen);
                break;
        }
    }

    public void Remove(SitizenView sitizen)
    {
        switch (sitizen.Type)
        {
            case SitizenViewType.Economy:
                _activeEconomySitizens.Remove(sitizen);
                break;
            case SitizenViewType.Religion:
                _activeReligionSitizens.Remove(sitizen);
                break;
            case SitizenViewType.Culture:
                _activeCultureSitizens.Remove(sitizen);
                break;
        }
    }

    public SitizenView GetRandomOfType(SitizenViewType type)
    {
        switch (type)
        {
            case SitizenViewType.Economy:
                return _activeEconomySitizens[Random.Range(0, _activeEconomySitizens.Count)];
            case SitizenViewType.Religion:
                return _activeReligionSitizens[Random.Range(0, _activeReligionSitizens.Count)];
            case SitizenViewType.Culture:
                return _activeCultureSitizens[Random.Range(0, _activeCultureSitizens.Count)];
        }

        return null;
    }
}
