using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Voronoi : MonoBehaviour
{

    public List<Vector2> points = new List<Vector2>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 30; i++)
        {
            points.Add(new Vector2(Random.Range(0, 30), Random.Range(0, 30)));
        }








    }

    // Update is called once per frame
    void Update()
    {
        
    }








    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.green;

            Handles.Label(points[i], i.ToString());
            Gizmos.DrawSphere(points[i], 0.1f);
        }
    }


}
