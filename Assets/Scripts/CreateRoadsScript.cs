using System.Collections.Generic;
using UnityEngine;

public class CreateRoadsScript : MonoBehaviour
{ 
    public GameObject roadSegment;
    private float segmentLength;
    private List<GameObject> roads = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        segmentLength = roadSegment.GetComponent<Renderer>().bounds.size[0];
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject road in roads)
        {
            Destroy(road);
        }

        roads = new List<GameObject>();
        GameObject[] houses = GameObject.FindGameObjectsWithTag("House");

        if (houses.Length > 1)
        {
            var housesToConnect = MinimumSpanningTree(houses);
            foreach (var housePair in housesToConnect)
            {
                ConnectHouses(housePair.Item1, housePair.Item2);
            }
        }
    }

    // Finds the minimum spanning tree in the fully connected graph between objs
    HashSet<(Transform, Transform)> MinimumSpanningTree(GameObject[] objs)
    {
        Transform root = objs[0].transform;
        var edgesToChooseFrom = new List<(Transform, Transform)>();
        var edges = new HashSet<(Transform, Transform)>();
        var edgeComparer = new EdgeComparer();

        // Fill edgesToChooseFrom with edges from the root
        for (int i = 1; i < objs.Length; ++i)
        {
            Transform h = objs[i].transform;
            edgesToChooseFrom.Add((h, root));
        }

        // Iteratively choose smallest edge to expand tree
        while (edgesToChooseFrom.Count > 0)
        {
            edgesToChooseFrom.Sort(edgeComparer);
            var bestEdge = edgesToChooseFrom[0];
            edges.Add(bestEdge);
            edgesToChooseFrom.RemoveAt(0);

            // Update edgesToChooseFrom
            for (int i = 0; i < edgesToChooseFrom.Count; ++i)
            {
                var newEdge = (edgesToChooseFrom[i].Item1, bestEdge.Item1);
                if (edgeComparer.Compare(newEdge, edgesToChooseFrom[i]) < 0)
                {
                    edgesToChooseFrom[i] = newEdge;
                }
            }
        }

        return edges;
    }

    // Connects two houses by roads
    void ConnectHouses(Transform h1, Transform h2)
    {
        Vector3 dir = h2.position - h1.position;
        int numOfSegments = (int) Mathf.Ceil(dir.magnitude / segmentLength);

        if (numOfSegments > 1)
        {
            float spacing = (dir.magnitude - segmentLength) / (numOfSegments - 1);

            for (int i = 0; i < numOfSegments; ++i)
            {
                GameObject road = Instantiate(
                    roadSegment,
                    h1.position + (segmentLength / 2 + i * spacing) * dir / dir.magnitude,
                    Quaternion.LookRotation(dir, h1.up));

                road.transform.Rotate(new Vector3(-90, 0, 0));
                roads.Add(road);
            }
        }
        else
        {
            GameObject road = Instantiate(
                roadSegment,
                h1.position + dir / 2,
                Quaternion.LookRotation(dir, h1.up));

            road.transform.Rotate(new Vector3(-90, 0, 0));
            roads.Add(road);
        }


    }

    private class EdgeComparer : IComparer<(Transform, Transform)>
    {
        public int Compare((Transform, Transform) e1, (Transform, Transform) e2)
        {
            return Vector3.Distance(e1.Item1.position, e1.Item2.position).CompareTo(Vector3.Distance(e2.Item1.position, e2.Item2.position));
        }
    }
}
