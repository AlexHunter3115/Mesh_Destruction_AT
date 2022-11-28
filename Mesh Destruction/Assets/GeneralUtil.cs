using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public static class GeneralUtil
{


    public static List<Triangle> RunTriangulation(List<Vector3> pointListRef, bool direction)
    {



        List<Vector2> pointList = new List<Vector2>();


        if (direction)
        {
            foreach (var passedPoint in pointListRef)
            {
                pointList.Add(new Vector2(passedPoint.x, passedPoint.y));
            }
        }
        else
        {
            foreach (var passedPoint in pointListRef)
            {
                pointList.Add(new Vector2(passedPoint.x, passedPoint.z));
            }
        }



        List<Triangle> triangulation = new List<Triangle>();

        Vector3 superTriangleA = new Vector3(10000, 10000, 10000);
        Vector3 superTriangleB = new Vector3(10000, 0, 10000);
        Vector3 superTriangleC = new Vector3(0, 10000, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));


        foreach (Vector3 point in pointList)
        {

            List<Triangle> badTriangles = new List<Triangle>();

            foreach (Triangle triangle in triangulation)
            {
                if (IspointInCircumcircle(triangle.a, triangle.b, triangle.c, point))
                {
                    badTriangles.Add(triangle);
                }
            }





            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in badTriangles)
            {
                foreach (Edge triangleEdge in triangle.edges)
                {
                    bool isShared = false;

                    foreach (Triangle otherTri in badTriangles)
                    {
                        if (otherTri == triangle) { continue; }

                        foreach (Edge otherEdge in otherTri.edges)
                        {
                            if (LineIsEqual(triangleEdge, otherEdge))
                            {
                                isShared = true;
                            }
                        }



                    }

                    if (isShared == false)
                    {
                        polygon.Add(triangleEdge);
                    }

                }
            }


            foreach (Triangle badTriangle in badTriangles)
            {
                triangulation.Remove(badTriangle);   // i think this is the issue here
            }

            foreach (Edge edge in polygon)
            {
                Triangle newTriangle = new Triangle(edge.edge[0], edge.edge[1], point);
                triangulation.Add(newTriangle);
            }

        }



        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            if (triangulation[i].HasVertex(superTriangleA) || triangulation[i].HasVertex(superTriangleB) || triangulation[i].HasVertex(superTriangleC))
            {
                triangulation.Remove(triangulation[i]);
            }

        }




        return triangulation;

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


    /// <summary>
    /// returns true if the point is in the circle
    /// </summary>
    public static bool IspointInCircumcircle(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {


        float ax_ = A[0] - D[0];
        float ay_ = A[1] - D[1];
        float bx_ = B[0] - D[0];
        float by_ = B[1] - D[1];
        float cx_ = C[0] - D[0];
        float cy_ = C[1] - D[1];



        if ((
            (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
            (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
            (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
        ) < 0)
        {
            return true;
        }

        else { return false; }

    }

    public static bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }





}




