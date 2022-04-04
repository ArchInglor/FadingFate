using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EarthSegment))]
public class EarthMover : MonoBehaviour
{
    public event Action EarthSegmentOutOfBounds;

    [SerializeField] private float _bound;
    private float _speed;
    private float _segmentWidth;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private void Start()
    {
        _segmentWidth = GetComponent<EarthSegment>().GetWidth();
    }

    private void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if(_speed >= 0)
        {
            if (transform.localPosition.x >= _bound)
            {
                EarthSegmentOutOfBounds?.Invoke();
                transform.localPosition = transform.localPosition + Vector3.left * _segmentWidth * 2;
            }
        }
        else
        {
            if (transform.localPosition.x <= -_bound)
            {
                EarthSegmentOutOfBounds?.Invoke();
                transform.localPosition = transform.localPosition + Vector3.right * _segmentWidth * 2;
            }
        }
        
    }
}
