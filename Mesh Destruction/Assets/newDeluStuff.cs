using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class newDeluStuff : MonoBehaviour
{
    public List<Vector3> points = new List<Vector3>();

    public bool view;

    List<GeneralUtil.Triangle> triangles = new List<GeneralUtil.Triangle>();


    [Header("positive means up")]
    public bool orientation;

    private void Start()
    {
        Mesh planeMesh = GetComponent<MeshFilter>().mesh;

        points = new List<Vector3>();




        points.Add(transform.TransformPoint(planeMesh.vertices[0]));
        points.Add(transform.TransformPoint(planeMesh.vertices[10]));
        points.Add(transform.TransformPoint(planeMesh.vertices[110]));
        points.Add(transform.TransformPoint(planeMesh.vertices[120]));

        Vector3 A = points[0];
        Vector3 B = points[1];
        Vector3 C = points[2];
        Vector3 D = points[3];




        // Ray ray = new Ray();

        // Vector3 dir = A - B;
        //ray.direction = dir;




        ////from A to B       top left to top right

        float perc = 0.25f;
        Vector3 interpolatedPosition = Vector3.Lerp(A, B, perc);
        points.Add(interpolatedPosition);


        perc = 0.66f;
        interpolatedPosition = Vector3.Lerp(A, B, perc);
        points.Add(interpolatedPosition);

        perc = 0.8f;
        interpolatedPosition = Vector3.Lerp(A, B, perc);
        points.Add(interpolatedPosition);




        ////from B to D        top right   to   bot right


        perc = 0.25f;
        interpolatedPosition = Vector3.Lerp(B, D, perc);
        points.Add(interpolatedPosition);

        perc = 0.66f;
        interpolatedPosition = Vector3.Lerp(B, D, perc);
        points.Add(interpolatedPosition);

        perc = 0.8f;
        interpolatedPosition = Vector3.Lerp(B, D, perc);
        points.Add(interpolatedPosition);



        ////from C to D        top right   to   bot right


        perc = 0.25f;
        interpolatedPosition = Vector3.Lerp(C, D, perc);
        points.Add(interpolatedPosition);

        perc = 0.66f;
        interpolatedPosition = Vector3.Lerp(C, D, perc);
        points.Add(interpolatedPosition);

        perc = 0.8f;
        interpolatedPosition = Vector3.Lerp(C, D, perc);
        points.Add(interpolatedPosition);



        ////from C to D        top right   to   bot right

        perc = 0.25f;
        interpolatedPosition = Vector3.Lerp(C, A, perc);
        points.Add(interpolatedPosition);


        perc = 0.66f;
        interpolatedPosition = Vector3.Lerp(C, A, perc);
        points.Add(interpolatedPosition);


        perc = 0.8f;
        interpolatedPosition = Vector3.Lerp(C, A, perc);
        points.Add(interpolatedPosition);



        for (int i = 0; i < 30; i++)
        {


            CalculateRandomPoint();

        }

        Debug.Log(points.Count);



        // true is when its -90 on the x       laying on the shit false
        triangles = GeneralUtil.RunTriangulation(points, orientation);

        Debug.Log(triangles.Count);

    }




    List<Vector3> EdgeVectors = new List<Vector3>();

    void CalculateEdgeVectors(int VectorCorner)
    {
        EdgeVectors.Clear();

        EdgeVectors.Add(points[3] - points[VectorCorner]);
        EdgeVectors.Add(points[1] - points[VectorCorner]);
    }

    public void CalculateRandomPoint()
    {
        int randomCornerIdx = Random.Range(0, 2) == 0 ? 0 : 2; //there is two triangles in a plane, which tirangle contains the random point is chosen
                                                               //corner point is chosen for triangles as the variable
        CalculateEdgeVectors(randomCornerIdx); //in case of transform changes edge vectors change too

        float u = Random.Range(0.0f, 1.0f);
        float v = Random.Range(0.0f, 1.0f);

        if (v + u > 1) //sum of coordinates should be smaller than 1 for the point be inside the triangle
        {
            v = 1 - v;
            u = 1 - u;
        }

        Vector3 RandomPoint = points[randomCornerIdx] + u * EdgeVectors[0] + v * EdgeVectors[1];

        points.Add(RandomPoint);


    }



    private void OnDrawGizmos()
    {



        if (view)
        {
            foreach (var tri in triangles)
            {
                if (orientation) 
                {
                    Debug.DrawLine(new Vector3(tri.edges[0].edge[0].x, tri.edges[0].edge[0].y, tri.edges[0].edge[0].z), new Vector3(tri.edges[0].edge[1].x, tri.edges[0].edge[1].y, tri.edges[0].edge[1].z), Color.green);
                    Debug.DrawLine(new Vector3(tri.edges[1].edge[0].x, tri.edges[1].edge[0].y, tri.edges[1].edge[0].z), new Vector3(tri.edges[1].edge[1].x, tri.edges[1].edge[1].y, tri.edges[1].edge[1].z), Color.green);
                    Debug.DrawLine(new Vector3(tri.edges[2].edge[0].x, tri.edges[2].edge[0].y, tri.edges[2].edge[0].z), new Vector3(tri.edges[2].edge[1].x, tri.edges[2].edge[1].y, tri.edges[2].edge[1].z), Color.green);

                }
                else 
                {
                
                    
                }



            }

        }
        else
        {
            Gizmos.color = Color.yellow;
            foreach (var vertex in points)
            {
                Gizmos.DrawSphere(vertex, 0.3f);

            }
        }


    }



}
