using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthGeneration : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private EarthSegment _segmentTemplate;

    private EarthSegment[] _segments = new EarthSegment[2];
    private float _secondSegmentOffset;

    private void Start()
    {
        _segments[0] = Instantiate(_segmentTemplate, transform.position, Quaternion.identity, transform);
        _segments[1] = Instantiate(_segmentTemplate, transform);
        foreach(var segment in _segments)
        {
            segment.Initialize();
        }
        _secondSegmentOffset = _segments[0].GetWidth();
        _segments[1].transform.position = transform.position + Vector3.left * _secondSegmentOffset * Mathf.Sign(_speed);
        var earthMovers = GetComponentsInChildren<EarthMover>();
        foreach (var earthMover in earthMovers)
        {
            earthMover.SetSpeed(_speed);
            earthMover.EarthSegmentOutOfBounds += OnEarthSegmentOutOfBound;
        }
    }

    private void OnEarthSegmentOutOfBound()
    {

    }
}
