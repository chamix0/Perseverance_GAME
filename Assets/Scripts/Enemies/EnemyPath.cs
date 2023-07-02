using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(7)]
public class EnemyPath : MonoBehaviour
{
    [SerializeField] private List<Transform> nodes;
    [SerializeField] private List<Vector2> paths;
    private Vector2[][] distances;

    private void Awake()
    {
        distances = new Vector2[nodes.Count][];
        for (int i = 0; i < nodes.Count; i++)
        {
            distances[i] = new Vector2[nodes.Count];
            for (int j = 0; j < nodes.Count; j++)
            {
                distances[i][j].y = -1;
                if (i == j)
                {
                    distances[i][j].y = j;
                    distances[i][j].x = 0f;
                }
                else
                {
                    distances[i][j].x = Mathf.Infinity;
                }
            }
        }
    }

    private void Start()
    {
        Bidirectionalize();
        InitDistances();
        // PrintDistances();
        for (int i = 0; i < nodes.Count; i++)
            Dijkstra(i);
        // PrintDistances();
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

    public int GetNextNode(int currentNode, int TargetNode)
    {
        return (int)distances[currentNode][TargetNode].y;
    }

    public int GetRandomNode()
    {
        return Random.Range(0, nodes.Count);
    }

    public int GetNumNodes()
    {
        return nodes.Count;
    }

    public Transform GetNodeTransform(int node)
    {
        return nodes[node];
    }

    private void Dijkstra(int nodeIni)
    {
        List<int> selectedNodes = new List<int>();
        selectedNodes.Add(nodeIni);
        List<int> pendNodes = InitPendNodes(nodeIni);
        while (pendNodes.Count > 0)
        {
            int candidate = SelectCandidate(pendNodes.ToArray(), nodeIni);
            selectedNodes.Add(candidate);
            pendNodes.Remove(candidate);
            UpdateDistances(candidate, nodeIni);
        }
    }

    private void UpdateDistances(int candidate, int nodeIni)
    {
        float distToCandidate = distances[nodeIni][candidate].x;
        for (int i = 0; i < nodes.Count; i++)
        {
            float distActual = distances[nodeIni][i].x;
            float distFromCandidate = distToCandidate + distances[candidate][i].x;
            distances[nodeIni][i].x = Mathf.Min(distActual, distFromCandidate);
            distances[nodeIni][i].y =
                distActual < distFromCandidate ? distances[nodeIni][i].y : distances[nodeIni][candidate].y;
        }
    }

    private int SelectCandidate(int[] penNodes, int nodeIni)
    {
        float minVAl = Mathf.Infinity;
        int nodeDest = nodeIni;
        foreach (var node in penNodes)
        {
            if (distances[nodeIni][node].x < minVAl)
            {
                nodeDest = node;
                minVAl = distances[nodeIni][node].x;
            }
        }

        if (nodeDest == nodeIni)
        {
            nodeDest = penNodes[0];
        }

        return nodeDest;
    }

    private void Bidirectionalize()
    {
        List<Vector2> newPaths = new List<Vector2>();

        foreach (var path in paths)
        {
            int node1 = (int)path.x;
            int node2 = (int)path.y;

            if (node1 != node2)
            {
                Vector2 invPath = new Vector2(node2, node1);
                if (!newPaths.Contains(path))
                    newPaths.Add(path);
                if (!newPaths.Contains(invPath))
                    newPaths.Add(invPath);
            }
        }

        paths = newPaths;
    }

    public int GetClosestNode(Vector3 enemyPos, int oldIndex)
    {
        int minIndex = oldIndex;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 nodePos = nodes[i].position;
            float dist = Vector3.Distance(enemyPos, nodePos);
            RaycastHit auxHit;
            if (Physics.Raycast(nodePos, enemyPos - nodePos, out auxHit,
                    dist))
                if (auxHit.transform.gameObject.layer == 11)
                {
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        minIndex = i;
                    }
                }
        }

        return minIndex;
    }

    public int GetClosestPlayerNode(Vector3 playerPos, int oldIndex)
    {
        int minIndex = oldIndex;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 nodePos = nodes[i].position;
            float dist = Vector3.Distance(playerPos, nodePos);
            RaycastHit auxHit;
            if (Physics.Raycast(nodePos, playerPos - nodePos, out auxHit,
                    dist))
                if (auxHit.transform.gameObject.layer == 9)
                {
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        minIndex = i;
                    }
                }
        }

        return minIndex;
    }

    public int GetFurthestNode(Vector3 playerPos, int oldIndex)
    {
        int nodeIni = GetClosestPlayerNode(playerPos, oldIndex);
        int maxIndex = nodeIni;
        float maxDistance = -Mathf.Infinity;
        for (int i = 0; i < nodes.Count; i++)
        {
            float dist = distances[nodeIni][i].x;
            if (dist > maxDistance)
            {
                maxDistance = dist;
                maxIndex = i;
            }
        }

        return maxIndex;
    }

    private void PrintDistances()
    {
        string cad = "";
        for (int i = 0; i < nodes.Count; i++)
        {
            cad += "nodo origen " + i + " [";
            for (int j = 0; j < nodes.Count; j++)
            {
                cad += " ( " + distances[i][j].x.ToString(".0") + " | " + (int)distances[i][j].y + " ) ";
            }

            cad += "]" + '\n';
        }

        print(cad);
    }

    private void InitDistances()
    {
        foreach (var path in paths)
        {
            int node1 = (int)path.x;
            int node2 = (int)path.y;
            float dist = Vector3.Distance(nodes[node1].position, nodes[node2].position);
            distances[node1][node2].x = dist;
            distances[node2][node1].x = dist;
            distances[node1][node2].y = node2;
            distances[node2][node1].y = node1;
        }
    }

    private List<int> InitPendNodes(int nodeIni)
    {
        List<int> aux = new List<int>();
        for (int i = 0; i < nodes.Count; i++)
            if (i != nodeIni)
                aux.Add(i);
        return aux;
    }
}