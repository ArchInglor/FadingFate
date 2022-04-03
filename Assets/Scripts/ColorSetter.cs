using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[AddComponentMenu("CoughTeam/ColorSetter")]
public class ColorSetter : MonoBehaviour
{
    [SerializeField] private PaletteColor _color;

    public PaletteColor Color { get => _color; set => UpdateColor(value); }
    
    public bool UpdateColor()
    {
        var compImg = GetComponent<Image>();
        if (compImg != null) 
        {
            compImg.color = PalettesSystem.instance.GetColor(_color);
            return true;
        }
        var compRend = GetComponent<SpriteRenderer>();
        if (compRend != null) 
        {
            compRend.color = PalettesSystem.instance.GetColor(_color);
            return true;
        }
        return false;

    }
    public bool UpdateColor(PaletteColor clr)
    {   
        _color = clr;
        return UpdateColor();
    }
}
