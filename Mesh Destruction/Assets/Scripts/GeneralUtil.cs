using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Random = UnityEngine.Random;

public static class GeneralUtil
{


    public static List<Triangle> RunTriangulation(List<Vector2> pointListRef, bool direction)
    {

        List<Vector2> pointList = new List<Vector2>();

        List<Triangle> triangulation = new List<Triangle>();
        // start witha  sup triangles
        Vector2 superTriangleA = new Vector2(10000, 10000);
        Vector2 superTriangleB = new Vector2(10000, 0);
        Vector2 superTriangleC = new Vector2(0, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));
        // every point is indside this triangle




        // for every point on the list
        foreach (Vector2 point in pointList)
        {




            // these are triangles to be eliminated
            List<Triangle> badTriangles = new List<Triangle>();






            //check for all the triangles that are available right now
            foreach (Triangle triangle in triangulation)
            {
                // is the point we are looping thoguth right now isndie that tri
                if (IspointInCircumcircle(triangle.a, triangle.b, triangle.c, point))
                {
                    // if so needs to deleted
                    badTriangles.Add(triangle);
                }
            }

            List<Edge> polygon = new List<Edge>();

            //for every triangle in bad triangls
            foreach (Triangle triangle in badTriangles)
            {
                //get the edge
                foreach (Edge triangleEdge in triangle.edges)
                {
                    bool isShared = false;

                    //other loop to loop with other tri

                    foreach (Triangle otherTri in badTriangles)
                    {
                        // not it self
                        if (otherTri == triangle) { continue; }


                        // for each edge in the other tri
                        foreach (Edge otherEdge in otherTri.edges)
                        {
                            bool test = false;

                            //  if any edges in the main triangles match with the other tri edges call everythign off, if they done mathc then add the tri edge o the main tri
                            if (LineIsEqual(triangleEdge, otherEdge))
                            {
                                isShared = true;
                                test = true;
                                break;
                            }

                            if (test)
                                break;
                            
                        }

                        if (isShared == false)
                        {
                            polygon.Add(triangleEdge);
                        }

                    }
                }

                //take out those bad triangles
                foreach (Triangle badTriangle in badTriangles)
                {
                    triangulation.Remove(badTriangle);   // i think this is the issue here
                }


                //from the possible edges create the tri with the looped point
                foreach (Edge edge in polygon)
                {
                    Triangle newTriangle = new Triangle(edge.edge[0], edge.edge[1], point);
                    triangulation.Add(newTriangle);
                }

            }


            

        }


        //get rid of the starting traingle reminants
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




    /// <summary>
    /// Gives a certin amoutn of point and it will return the triangle array and the vector array to create the convex hull
    /// </summary>
    /// <param name="points"></param>
    /// <returns>first is the vertices then the tringles index</returns>
    public static Tuple<List<Vector3>, List<int>> IncrementalConvex(List<Vector3> points) 
    {

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
            if (iter > 100)
            {
                Debug.Log($"exit on the iter");
                //destroy = true;
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

            List<Edge> interestedEdges = new List<Edge>();
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










        return Tuple.Create(vertecies, triangle);
    
    
    
    }




    #region ToDel

    public static Vector3 Circumcentre(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Calculate plane for edge AB
        Vector3 planeABNormal = b - a;
        Vector3 planeABPoint = Vector3.Lerp(a, b, 0.5f);

        // Calculate plane for edge AC
        Vector3 planeACNormal = c - a;
        Vector3 planeACPoint = Vector3.Lerp(a, c, 0.5f);

        // Calculate plane for edge BD
        Vector3 planeBDNormal = d - b;
        Vector3 planeBDPoint = Vector3.Lerp(b, d, 0.5f);

        // Calculate plane for edge CD
        Vector3 planeCDNormal = d - c;
        Vector3 planeCDPoint = Vector3.Lerp(c, d, 0.5f);

        // Calculate line that is the plane-plane intersection between AB and AC
        Vector3 linePoint1;
        Vector3 lineDirection1;

        // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
        PlanePlaneIntersection(out linePoint1, out lineDirection1, planeABNormal, planeABPoint, planeACNormal, planeACPoint);

        Vector3 linePoint2;
        Vector3 lineDirection2;

        PlanePlaneIntersection(out linePoint2, out lineDirection2, planeBDNormal, planeBDPoint, planeCDNormal, planeCDPoint);

        // Calculate the point that is the plane-line intersection between the above line and CD
        Vector3 intersection;

        // Floating point inaccuracy often causes these two lines to not intersect, in that case get the two closest points on each line 
        // and average them
        // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
        if (!LineLineIntersection(out intersection, linePoint1, lineDirection1, linePoint2, lineDirection2))
        {
            Vector3 closestLine1;
            Vector3 closestLine2;

            // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
            ClosestPointsOnTwoLines(out closestLine1, out closestLine2, linePoint1, lineDirection1, linePoint2, lineDirection2);

            // Intersection is halfway between the closest two points on lines
            intersection = Vector3.Lerp(closestLine2, closestLine2, 0.5f);
        }

        return intersection;
    }

    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else
        {
            return false;
        }
    }

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public static bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
    {

        linePoint = Vector3.zero;
        lineVec = Vector3.zero;

        //We can get the direction of the line of intersection of the two planes by calculating the 
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet. We need a point for that to go with the line vector.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.		
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float denominator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
        if (Mathf.Abs(denominator) > 0.006f)
        {

            Vector3 plane1ToPlane2 = plane1Position - plane2Position;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    public static bool IspointInCircumsphere(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Vector3 E)
    {
        Vector3 center = GeneralUtil.Circumcentre(A, B, C, D);

        float radiusCenA = Vector3.Distance(center, A);
        float dist = Vector3.Distance(center, E);

        if (dist > radiusCenA)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion




    





}




