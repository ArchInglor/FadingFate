using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer), typeof(ColorSetter))]
public class BuildingView : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private ColorSetter _colorSetter;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _colorSetter = GetComponent<ColorSetter>();
    }

    public void SetSprite(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }


}
