using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System.Linq;

public class IncrementalAlgo : MonoBehaviour
{


    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> pointsTest = new List<Vector3>();
    public Material mat;
    public List<tetraDeluTrig.Triangle> triangles = new List<tetraDeluTrig.Triangle>();


    public bool test = false;
   // public bool type = false;
    
    void Start()
    {
        RunIterationAlgo();
    }








    private void RunIterationAlgo() 
    {


        points.Clear();
        triangles.Clear();




        if (test)
        {
            points = pointsTest;
        }
        else 
        {
            for (int i = 0; i < 30; i++)
            {
                points.Add(new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), Random.Range(-15, 15)));
            }
        }


        points = pointsTest;



        List<int> triangleIndex = new List<int>();

        List<Vector3> vertex = new List<Vector3>();


        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();


        triangles.Add(new tetraDeluTrig.Triangle(points[0], points[1], points[2]));
        triangles.Add(new tetraDeluTrig.Triangle(points[3], points[2], points[1]));
        triangles.Add(new tetraDeluTrig.Triangle(points[0], points[2], points[3]));
        triangles.Add(new tetraDeluTrig.Triangle(points[1], points[0], points[3]));


        points.RemoveAt(0);
        points.RemoveAt(0);
        points.RemoveAt(0);
        points.RemoveAt(0);


        int iter = 0;
        bool destroy = false;


        while (points.Count > 1)
        {
            if (iter > 100)
            {
                Debug.Log($"exit on the iter");
                destroy = true;
                break;
            }
            iter++;




            int randomIndex = 0;

            for (int i = points.Count; i-- > 0;)
            {


                bool insideHull = false;
                foreach (var tri in triangles)
                {
                    Plane plane = new Plane(tri.a, tri.b, tri.c);

                    if (plane.GetSide(points[i]))   //outside
                    {
                        insideHull = true;
                        break;
                    }
                    else   //inside
                    {

                    }
                }

                if (!insideHull)
                {
                    points.RemoveAt(i);
                }
                else
                {

                }
            }

            randomIndex = Random.Range(0, points.Count);
         
            List<int> interestedTris = new List<int>();



            for (int i = 0; i < triangles.Count; i++)
            {
                Plane plane = new Plane(triangles[i].a, triangles[i].b, triangles[i].c);

                if (plane.GetSide(points[randomIndex]))
                {
                    interestedTris.Add(i);
                }
            }

            interestedTris.Sort();
            interestedTris.Reverse();

            List<tetraDeluTrig.Edge> interestedEdges = new List<tetraDeluTrig.Edge>();
            List<tetraDeluTrig.Triangle> rejectedTrigs = new List<tetraDeluTrig.Triangle>();

            foreach (var removeIdx in interestedTris)
            {
                rejectedTrigs.Add(triangles[removeIdx]);
                triangles.RemoveAt(removeIdx);
            }

            if (rejectedTrigs.Count > 1)
            {
                List<tetraDeluTrig.Edge> allEdges = new List<tetraDeluTrig.Edge>();
                List<int> delInt = new List<int>();


                foreach (var tri in rejectedTrigs)   // this adds all the possible angles
                {
                    allEdges.Add(tri.edges[0]);
                    allEdges.Add(tri.edges[1]);
                    allEdges.Add(tri.edges[2]);
                }

                for (int i = 0; i < allEdges.Count; i++)
                {
                    bool same = false;
                    int sameIDX = 0;
                    for (int j = 0; j < allEdges.Count; j++)
                    {

                        if (i == j)
                        {
                            continue;
                        }
                        else
                        {
                            if (LineIsEqual(allEdges[i], allEdges[j]))
                            {// the should techincally only be one
                                same = true;
                                sameIDX = j;
                                break;
                            }
                        }
                    }

                    if (same)
                    {
                        if (!delInt.Contains(i))
                        {

                            delInt.Add(i);
                            delInt.Add(sameIDX);
                        }
                    }
                }


                delInt.Sort();
                delInt.Reverse();

                foreach (var idx in delInt)
                {
                    if (idx >= allEdges.Count) { }
                    else 
                    {
                        allEdges.RemoveAt(idx);
                    }
                }

                foreach (var edges in allEdges)
                {
                    triangles.Add(new tetraDeluTrig.Triangle(edges.edge[0], edges.edge[1], points[randomIndex]));
                }
            }
            else
            {
                foreach (var edges in rejectedTrigs[0].edges)
                {
                    triangles.Add(new tetraDeluTrig.Triangle(edges.edge[0], edges.edge[1], points[randomIndex]));
                }

            }

        }




        if (destroy) 
        {
            Debug.Log("<color=red> This iter did not work </color>");
        }

        foreach (var tri in triangles)
        {
            triangleIndex.Add(vertex.Count);
            vertex.Add(tri.a);
            triangleIndex.Add(vertex.Count);
            vertex.Add(tri.b);
            triangleIndex.Add(vertex.Count);
            vertex.Add(tri.c);
        }


        mesh.vertices = vertex.ToArray();
        mesh.triangles = triangleIndex.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        this.GetComponent<MeshRenderer>().material = mat;
        points.Clear();

    }







    // to check
    public bool LineIsEqual(tetraDeluTrig.Edge A, tetraDeluTrig.Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
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


    void Update()
    {


        //if (test) 
        //{
        //    test = false;
        //    RunIterationAlgo();
        //}


        //foreach (var tri in triangles)
        //{
        //    foreach (var edge in tri.edges)
        //    {
        //        //Debug.DrawLine(edge.edge[0], edge.edge[1], Color.red);
        //    }
        //}
    }

}
