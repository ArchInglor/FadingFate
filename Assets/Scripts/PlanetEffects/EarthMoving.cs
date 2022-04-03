using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EarthSegment))]
public class EarthMoving : MonoBehaviour
{
    public event Action EarthSegmentOutOfBounds;

    [SerializeField] private float _speed;
    [SerializeField] private float _bound;
    private float _segmentWidth;

    private void Start()
    {
        _segmentWidth = GetComponent<EarthSegment>().GetWidth();
    }

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if(transform.localPosition.x >= _bound)
        {
            EarthSegmentOutOfBounds?.Invoke();
            transform.localPosition = transform.localPosition + Vector3.left * _segmentWidth * 2;
        }
    }
}
