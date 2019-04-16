using System.Collections.Generic;
using UnityEngine;

public class CreateRoadsScript : MonoBehaviour
{ 
    public GameObject roadSegment;
    private float segmentLength = 5.0F;
    private List<GameObject> roads;

    // Start is called before the first frame update
    void Start()
    {
        roads = new List<GameObject>();
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

        for (int i = 0; i < houses.Length; ++i)
        {
            for (int j = i + 1; j < houses.Length; ++j)
            {
                ConnectHouses(houses[i], houses[j]);
            }
        }

    }

    // Connects two houses by roads
    void ConnectHouses(GameObject h1, GameObject h2)
    {
        Vector3 p1 = h1.transform.position;
        Vector3 p2 = h2.transform.position;
        float dist = Vector3.Distance(p1, p2);
        int numOfSegments = (int)(dist / segmentLength);
    }
}
