using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere
{
    private Vector3 center;
    private float radius;

    public Sphere(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public void SetCenter(Vector3 center)
    {
        this.center = center;
    }

    public Vector3[] GetPolesFromRay(Ray ray)
    {
        float d = -(ray.direction.x * center.x * ray.direction.y * center.y + ray.direction.z * center.z);
        float y = center.y;
        float z = 10;
        float x = (-ray.direction.y * center.y - ray.direction.z * z - d) / ray.direction.x;

        Vector3 otherPoint = new Vector3(x, y, z);
        Ray aux = new Ray(center, otherPoint - center);
        return new Vector3[] { aux.GetPoint(radius), aux.GetPoint(-radius) };
    }

    public Vector3 GetCenter()
    {
        return center;
    }
}