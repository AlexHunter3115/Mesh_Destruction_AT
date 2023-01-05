using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public static class GeneralUtil
{




    /// <summary>
    /// Gives a certin amoutn of point and it will return the triangle array and the vector array to create the convex hull
    /// </summary>
    /// <param name="points"></param>
    /// <returns>first is the vertices then the tringles index</returns>
    public static Tuple<List<Vector3>, List<int>> IncrementalConvex(List<Vector3> points) 
    {

        int time = Environment.TickCount & Int32.MaxValue;


        Debug.Log(points.Count);

        var vertecies = new List<Vector3>();
        var triangle = new List<int>();

       var triangles = new List<Triangle>();


        triangles.Add(new Triangle(points[0], points[1], points[2]));
        triangles.Add(new Triangle(points[3], points[2], points[1]));
        triangles.Add(new Triangle(points[0], points[2], points[3]));
        triangles.Add(new Triangle(points[1], points[0], points[3]));


        points.RemoveAt(0);
        points.RemoveAt(0);
        points.RemoveAt(0);
        points.RemoveAt(0);


        int iter = 0;
        bool destroy = false;


        while (points.Count > 1)
        {
            if (iter > 120)
            {
                Debug.Log($"exit on the iter");
                //destroy = true;
                break;
            }
            iter++;


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
                }

                if (!insideHull)
                {
                    points.RemoveAt(i);
                }
            }

            int randomIndex = Random.Range(0, points.Count-1);

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


            List<Triangle> rejectedTrigs = new List<Triangle>();

            foreach (var removeIdx in interestedTris)
            {
                rejectedTrigs.Add(triangles[removeIdx]);
                triangles.RemoveAt(removeIdx);
            }

            if (rejectedTrigs.Count > 1)
            {
                List<Edge> allEdges = new List<Edge>();
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
                    triangles.Add(new Triangle(edges.edge[0], edges.edge[1], points[randomIndex]));
                }
            }
            else
            {
                foreach (var edges in rejectedTrigs[0].edges)
                {
                    triangles.Add(new Triangle(edges.edge[0], edges.edge[1], points[randomIndex]));
                }
            }
        }




        //if (destroy)
        //{
        //    Debug.Log("<color=red> This iter did not work </color>");
        //}

        foreach (var tri in triangles)
        {
            triangle.Add(vertecies.Count);
            vertecies.Add(tri.a);
            triangle.Add(vertecies.Count);
            vertecies.Add(tri.b);
            triangle.Add(vertecies.Count);
            vertecies.Add(tri.c);
        }






        int timerEnd = Environment.TickCount & Int32.MaxValue;

        Debug.Log($"<color=yellow>Performance: This operation took {timerEnd - time} ticks</color>");



        return Tuple.Create(vertecies, triangle);
    
    
    
    }



    public static List<Vector3>[] VoronoiDivision(List<Vector3> verticesPoints, int voronoiPoints) 
    {

        //need to check that the voronoi points isnt larger than the giving vertpoints


        List<Vector3> choosenPoints = new List<Vector3>();


        while (choosenPoints.Count != voronoiPoints) //decided the points
        {
            int ranIdx = Random.Range(0, verticesPoints.Count - 1);

            if (!choosenPoints.Contains(verticesPoints[ranIdx])) 
                choosenPoints.Add(verticesPoints[ranIdx]);

        }


        var listOfVoronoi = new List<Vector3>[choosenPoints.Count];
        for (int i = 0; i < listOfVoronoi.Length; i++)
        {
            listOfVoronoi[i] = new List<Vector3>();
        }

        foreach (var vertice in verticesPoints)
        {
            int closestIDx = 0;
            float dist = 99999;

            for (int i = 0; i < choosenPoints.Count; i++)
            {
                if (Vector3.Distance(choosenPoints[i], vertice) <= dist) 
                {
                    dist = Vector3.Distance(choosenPoints[i], vertice);
                    closestIDx = i;
                }
            }

            listOfVoronoi[closestIDx].Add(vertice);
        }




        return listOfVoronoi;


    }


    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Edge[] edges = new Edge[3];

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;


            this.edges[0] = new Edge(a, b);
            this.edges[1] = new Edge(b, c);
            this.edges[2] = new Edge(c, a);
        }


        public bool HasVertex(Vector3 point)
        {
            if (a == point || b == point || c == point) { return true; }
            else { return false; }
        }

    }

    public class Edge
    {
        public Vector3[] edge = new Vector3[2];

        public Edge(Vector3 a, Vector3 b)
        {
            edge[0] = a;
            edge[1] = b;
        }

    }


    public static bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }


}




