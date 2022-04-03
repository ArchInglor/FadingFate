using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Haze;


[RequireComponent(typeof(Collider2D))]
public class RandomPointPicker : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private Island[] _islands;

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();
        _islands = new Island[_collider.pathCount];

        for (int i = 0; i < _islands.Length; i++)
        {
            Vector2[] points = _collider.GetPath(i);
            var triangles = Triangulator.Triangulate(points.ToList(), 1);
            _islands[i] = new Island(triangles);
        }
    }

    public Vector2 GetRandomPointInCollider()
    {
        Vector2 resultPoint = new Vector2();

        Island island = GetRandomIsland();

        Triangulator.Triangle triangle = island.GetRandomTriangle();

        Vector2 center = triangle.Center;

        Vector2 point1 = Vector2.zero;
        Vector2 point2 = Vector2.zero;
        switch (Random.Range(0, 3))
        {
            case 0:
                point1 = triangle.a;
                point2 = triangle.b;
                break;
            case 1:
                point1 = triangle.b;
                point2 = triangle.c;
                break;
            case 2:
                point1 = triangle.a;
                point2 = triangle.c;
                break;
        }

        Vector2 randomBetween2Vector = RandomPointBetween2Points(point1, point2);
        resultPoint = RandomPointBetween2Points(randomBetween2Vector, center);

        return resultPoint;
    }

    private Island GetRandomIsland()
    {
        return _islands[Random.Range(0, _islands.Length)];
    }

    private static Vector3 RandomPointBetween2Points(Vector3 start, Vector3 end)
    {
        return (start + Random.Range(0f, 1f) * (end - start));
    }
}

public struct Island
{
    private List<Triangulator.Triangle> _triangles;

    public Island(List<Triangulator.Triangle> triangles)
    {
        _triangles = triangles;
    }

    public Triangulator.Triangle GetRandomTriangle()
    {
        return _triangles[Random.Range(0, _triangles.Count)];
    }
}
