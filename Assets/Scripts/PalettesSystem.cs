using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("CoughTeam/PalettesSystem")]
public class PalettesSystem : MonoBehaviour
{
    public static PalettesSystem instance = null;
    [SerializeField] private int _currentPalette;
    [SerializeField] private List<Palette> _palettes;

    public int CurrentPalette { get => _currentPalette; set => _currentPalette = SetCurrentPalette(value); }
    private int SetCurrentPalette(int val)
    {
        if (val > 0) 
        {
            if (val < _palettes.Count) return val;
            return _palettes.Count-1;
        }
        return 0;
    }
    public List<Palette> Palettes { get => _palettes; set => _palettes = value; }


    // Метод, выполняемый при старте игры
    void Awake () 
    {
        // Теперь, проверяем существование экземпляра
        if (instance == null) 
        { 
            // Экземпляр менеджера был найден
            instance = this; // Задаем ссылку на экземпляр объекта
        }
        else if(instance == this)
        { 
            // Экземпляр объекта уже существует на сцене
            Destroy(gameObject); // Удаляем объект
        }
        DontDestroyOnLoad(gameObject);

        // И запускаем собственно инициализатор
        InitializeManager();
        }

    // Метод инициализации менеджера
    private void InitializeManager()
    {
        ColorSetter[] components = GameObject.FindObjectsOfType<ColorSetter>();
        foreach (ColorSetter comp in components)
        {
            comp.UpdateColor();
        }
    }

    public Color GetColor(PaletteColor color) 
    {
        if (color == PaletteColor.colorCulture) return _palettes[_currentPalette].colorCulture;
        else if (color == PaletteColor.colorEconomy) return _palettes[_currentPalette].colorEconomy;
        else if (color == PaletteColor.colorReligion) return _palettes[_currentPalette].colorReligion;
        else if (color == PaletteColor.colorEarth) return _palettes[_currentPalette].colorEarth;
        else if (color == PaletteColor.colorWater) return _palettes[_currentPalette].colorWater;
        else if (color == PaletteColor.colorSpace) return _palettes[_currentPalette].colorSpace;
        return Color.magenta;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}