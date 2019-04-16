using System.Collections.Generic;
using UnityEngine;

public class CreateRoadsScript : MonoBehaviour
{ 
    public GameObject roadSegment;
    private float segmentLength;
    private List<GameObject> roads;

    // Start is called before the first frame update
    void Start()
    {
        roads = new List<GameObject>();
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

        if (houses.Length >= 2)
        {
            ConnectHouses(houses[0].transform, houses[1].transform);
        }

    }

    // Finds the minimum spanning tree in the fully connected graph between nodes
    void MinimumSpanningTree(GameObject[] nodes)
    {
        foreach (GameObject node in nodes)
        {
            Debug.Log("Hej");
        }
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
}
