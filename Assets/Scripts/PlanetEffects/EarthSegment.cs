using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EarthSegment : MonoBehaviour
{
    private Sprite _sprite;

    public void Initialize()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public float GetWidth()
    {
        return _sprite.bounds.size.x;
    }
}
