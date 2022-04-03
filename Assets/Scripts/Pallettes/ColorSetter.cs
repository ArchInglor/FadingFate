using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("CoughTeam/ColorSetter")]
public class ColorSetter : MonoBehaviour
{
    [SerializeField] private PaletteColor _color;
    [SerializeField] private float _hueOffset;
    [SerializeField] private float _brightness;
    
    public bool UpdateColor()
    {
        float h, s, v;
        var clr = PalettesSystem.instance.GetColor(_color);
        Color.RGBToHSV(clr, out h, out s, out v);
        h += _hueOffset;    
        v += _brightness;    
        clr = Color.HSVToRGB(h, s, v);

        var compImg = GetComponent<Image>();
        if (compImg != null) 
        {
            compImg.color = clr;
            return true;
        }
        var compRend = GetComponent<SpriteRenderer>();
        if (compRend != null) 
        {
            compRend.color = clr;
            return true;
        }
        return false;

    }
    public bool UpdateColor(PaletteColor clr)
    {   
        _color = clr;
        return UpdateColor();
    }

    private void OnEnable() {
        if (PalettesSystem.instance != null) UpdateColor();
    }
}
