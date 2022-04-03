using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthMoving : MonoBehaviour
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
        _segments[1].transform.position = transform.position + Vector3.right * _secondSegmentOffset * Mathf.Sign(_speed);
    }
}
