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
    private Text _text;
    private Camera _camera;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _renderer = GetComponent<SpriteRenderer>();
        _text = GetComponent<Text>();
    }

    public bool UpdateColor()
    {
        float h, s, v;
        var color = PalettesSystem.instance.GetColor(_color);
        Color.RGBToHSV(color, out h, out s, out v);
        h += _hueOffset;    
        v += _brightness;    
        color = Color.HSVToRGB(h, s, v);

        if (_image != null) 
        {
            _image.color = color;
            return true;
        }
        else 
        {
            if (_renderer != null) 
            {
                _renderer.color = color;
                return true;
            }
            else 
            {
                if (_text != null) 
                {
                    _text.color = color;
                    return true;
                }
                else 
                {
                    if (_camera != null) 
                    {
                        _camera.backgroundColor = color;
                        return true;
                    }
                }
            }
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
