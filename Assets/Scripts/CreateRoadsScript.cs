using System.Collections.Generic;
using UnityEngine;

public class CreateRoadsScript : MonoBehaviour
{ 
    public GameObject roadSegment;
    public GameObject carPrefab;
    private float segmentLength;
    private List<GameObject> roads = new List<GameObject>();
    private List<Car> cars = new List<Car>();

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
            var connectedHouses = new HashSet<(GameObject, GameObject)>();

            // Update car positions and connect houses of those cars
            for (int i = cars.Count - 1; i >= 0; --i)
            {
                Car car = cars[i];
                bool foundRoad = false;

                foreach (var housePair in housesToConnect)
                {
                    if ((housePair.Item1 == car.destination && housePair.Item2 == car.source) ||
                        (housePair.Item2 == car.destination && housePair.Item1 == car.source))
                    {
                        ConnectHouses(housePair.Item1.transform, housePair.Item2.transform);
                        connectedHouses.Add(housePair);

                        car.Step();
                        foundRoad = true;
                        break;
                    }
                }

                if (!foundRoad)
                {
                    // This car no longer has a road to drive on :(
                    cars.Remove(car);
                    Destroy(car.car);
                }
            }

            // Connect houses with no car, and make them a car
            housesToConnect.ExceptWith(connectedHouses);
            foreach (var housePair in housesToConnect)
            {
                Car car = new Car(carPrefab, housePair.Item1, housePair.Item2);
                cars.Add(car);
                ConnectHouses(housePair.Item1.transform, housePair.Item2.transform);
            }
        }
    }

    // Finds the minimum spanning tree in the fully connected graph between objs
    HashSet<(GameObject, GameObject)> MinimumSpanningTree(GameObject[] objs)
    {
        GameObject root = objs[0];
        var edgesToChooseFrom = new List<(GameObject, GameObject)>();
        var edges = new HashSet<(GameObject, GameObject)>();
        var edgeComparer = new EdgeComparer();

        // Fill edgesToChooseFrom with edges from the root
        for (int i = 1; i < objs.Length; ++i)
        {
            GameObject h = objs[i];
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

    private class EdgeComparer : IComparer<(GameObject, GameObject)>
    {
        public int Compare((GameObject, GameObject) e1, (GameObject, GameObject) e2)
        {
            return Vector3.Distance(e1.Item1.transform.position, e1.Item2.transform.position).CompareTo(Vector3.Distance(e2.Item1.transform.position, e2.Item2.transform.position));
        }
    }

    private class Car
    {
        public GameObject car;
        public GameObject destination;
        public GameObject source;
        public float progress = 0.0F;

        public Car(GameObject carPrefab, GameObject destinationObj, GameObject sourceObj)
        {
            destination = destinationObj;
            source = sourceObj;
            Vector3 dir = destination.transform.position - source.transform.position;
            car = Instantiate(
                carPrefab,
                source.transform.position,
                Quaternion.LookRotation(dir, source.transform.up));
        }

        public void Step()
        {
            progress += 0.01F;

            if (progress >= 1.0F)
            {
                SwitchDirection();
            }

            Vector3 dir = destination.transform.position - source.transform.position;
            car.transform.position = source.transform.position + dir * progress;
            car.transform.rotation = Quaternion.LookRotation(dir, source.transform.up);
        }

        private void SwitchDirection()
        {
            GameObject temp = destination;
            destination = source;
            source = temp;
            progress = 0.0F;
        }
    }
}
