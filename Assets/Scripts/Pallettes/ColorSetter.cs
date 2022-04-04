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

    private Image _image;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public bool UpdateColor()
    {
        float h, s, v;
        var clr = PalettesSystem.instance.GetColor(_color);
        Color.RGBToHSV(clr, out h, out s, out v);
        h += _hueOffset;    
        v += _brightness;    
        clr = Color.HSVToRGB(h, s, v);

        if (_image != null) 
        {
            _image.color = clr;
            return true;
        }

        if (_renderer != null) 
        {
            _renderer.color = clr;
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
