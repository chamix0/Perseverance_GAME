using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class EnemyPath : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes;
    [SerializeField] private List<Vector2> paths;

    private void Awake()
    {
    }

    private void Start()
    {
    }

    private void OnDrawGizmos()
    {
        foreach (var pos in nodes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos.position, 0.2f);
        }


        foreach (var path in paths)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(nodes[(int)path.x].position, nodes[(int)path.y].position);
            Ray ray = new Ray(nodes[(int)path.x].position, nodes[(int)path.y].position - nodes[(int)path.x].position);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(
                ray.GetPoint(Vector3.Distance(nodes[(int)path.x].position, nodes[(int)path.y].position) - 0.4f),
                0.1f);
        }
    }
}


