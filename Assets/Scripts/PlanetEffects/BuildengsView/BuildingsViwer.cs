using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RandomPointPicker))]
public class BuildingsViwer : MonoBehaviour
{
    [SerializeField] private BuildingView _buildingViewTemplate;
    private BuildingsSpritesMap _buildingsSpritesMap;

    private PoolMono<BuildingView> _buildingsViewsPool;

    private RandomPointPicker _randomPointPicker;

    private void Awake()
    {
        _randomPointPicker = GetComponent<RandomPointPicker>();
    }

    public void SpawnBuilding(BuildingType type)
    {
        Sprite sprite;

        switch (type)
        {
            case BuildingType.EconomyE:
                sprite = _buildingsSpritesMap.EE;
                break;
            case BuildingType.EconomyR:
                sprite = _buildingsSpritesMap.ER;
                break;
            case BuildingType.EconomyC:
                sprite = _buildingsSpritesMap.EC;
                break;
            case BuildingType.ReligionE:
                sprite = _buildingsSpritesMap.RE;
                break;
            case BuildingType.ReligionR:
                sprite = _buildingsSpritesMap.RR;
                break;
            case BuildingType.ReligionC:
                sprite = _buildingsSpritesMap.RC;
                break;
            case BuildingType.CultureE:
                sprite = _buildingsSpritesMap.CE;
                break;
            case BuildingType.CultureR:
                sprite = _buildingsSpritesMap.CR;
                break;
            case BuildingType.CultureC:
                sprite = _buildingsSpritesMap.CC;
                break;
        }

        var buildingView = _buildingsViewsPool.GetFreeElement();
        buildingView.transform.position = _randomPointPicker.GetRandomPointInCollider();
    }

    [System.Serializable]
public struct BuildingsSpritesMap
{
    public Sprite EE;
    public Sprite ER;
    public Sprite EC;
    public Sprite RE;
    public Sprite RR;
    public Sprite RC;
    public Sprite CE;
    public Sprite CR;
    public Sprite CC;
}
}