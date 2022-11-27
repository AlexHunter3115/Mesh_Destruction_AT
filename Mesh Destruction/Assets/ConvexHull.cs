
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.UI.Image;

public class ConvexHull : MonoBehaviour
{
    /*
     * get 3 points that share the same plane 
     * 
     * this plane also needs to have no other points on the other side can do this with the getSide plane shits
     * 
     * 
     * 
     * 
     * it all has to do with angles
     * by what i understad its create a tirangle   then pick one edge of that triangle and check the angle of that edge in respect to all the other points, the one which makes the biggest angle is the next vertex so then
     * add the vertices of that edge and the picked vertec create triangle and keep going
     * 
     * same edge and stuff like that can be stolen from delu algo
     * 
     */
    [SerializeField]
    public List<Vector3> points = new List<Vector3>();
    public List<point> pointsList = new List<point>();

    [SerializeField]
    List<TriangleConvexHull> triangles = new List<TriangleConvexHull>();




    public float angle = 0;



    // Start is called before the first frame update
    void Start()
    {

        // we need to get the first triangle 
        triangles = new List<TriangleConvexHull>();





        // this all works
        int iter = 0;
        bool leave = false;

        while (!leave) 
        {
            foreach (var vertex1 in pointsList)
            {
                if (leave) 
                {
                    break;
                }


                foreach (var vertex2 in pointsList)
                {


                    if (leave)
                    {
                        break;
                    }

                    if (vertex1 == vertex2) { continue; }

                    foreach (var vertex3 in pointsList)
                    {

                        if (leave)
                        {
                            break;
                        }
                        if (vertex2 == vertex3 || vertex1 == vertex3) { continue; }


                        Plane extremePlane = new Plane(vertex1.pointCoord, vertex2.pointCoord, vertex3.pointCoord);

                        bool trueExtreme = true;      //true if this is the extreme one 

                        foreach (var vertex4 in pointsList)
                        {
                            if (vertex2 == vertex4 || vertex1 == vertex4 || vertex3 == vertex4) { continue; }
                            

                            if (extremePlane.GetSide(vertex4.pointCoord)) 
                            {
                                trueExtreme = false;
                                break;
                            }
                        }

                        if (trueExtreme) 
                        {
                            triangles.Add(new(vertex1.pointCoord, vertex2.pointCoord, vertex3.pointCoord));
                            leave = true;
                            vertex1.choosen = true;
                            vertex2.choosen = true;
                            vertex3.choosen = true;
                            break;
                        }
                    }
                }
            }
            iter++;
        }



















        for (int i = 0; i < triangles.Count; i++)  // for every triangle   triangles.Count
        {

            if (iter >= 20) { break; }
            for (int j = 0; j < triangles[i].edges.Length; j++)   //check every edge
            {

                Vector3 alonevertex = Vector3.zero;   // get the alone vertex, with this i can get the dir of tringle so i can work out the perfect angle?????

                if (triangles[i].edges[j].edge[0] != triangles[i].a && triangles[i].edges[j].edge[1] != triangles[i].a) 
                {
                    alonevertex = triangles[i].a;
                }
                else if (triangles[i].edges[j].edge[0] != triangles[i].b && triangles[i].edges[j].edge[1] != triangles[i].b) 
                {
                    alonevertex = triangles[i].b;
                }
                else if (triangles[i].edges[j].edge[0] != triangles[i].c && triangles[i].edges[j].edge[1] != triangles[i].c)
                {
                    alonevertex = triangles[i].c;
                }




                Vector3 midpoint = Vector3.Lerp(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1], 0.5f);  // get the mid point of the edge


                point nextPoint = null;


                foreach (var point in pointsList)  //go through all the points
                {
                    if (point.pointCoord == triangles[i].a || point.pointCoord == triangles[i].b || point.pointCoord == triangles[i].c) { continue; }   //if the point is it self conntinue




                    var dirTri = alonevertex - midpoint;   // direction of the triangle

                    var dirNextPoint = point.pointCoord - midpoint;

                    if (nextPoint == null) 
                    {
                        nextPoint = point;
                    }
                    else 
                    {
                        if (Vector3.Angle(dirTri,dirNextPoint) > Vector3.Angle(dirTri, nextPoint.pointCoord - midpoint)) { nextPoint = point; }
                    }
                }
                bool exists = false;



                foreach (var tri in triangles)
                {
                    if (tri.HasVertex(triangles[i].edges[j].edge[0]) && tri.HasVertex(triangles[i].edges[j].edge[1]) && tri.HasVertex(nextPoint.pointCoord))
                    {
                        exists = true;
                    }
                }

                if (!exists)
                        triangles.Add(new TriangleConvexHull(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1], nextPoint.pointCoord));

            }
        }


        Debug.Log(triangles.Count);











        ////iter = 0;
        //for (int i = 0; i < triangles.Count; i++)
        //{

        //    Plane triPlane = new Plane(triangles[i].a, triangles[i].b, triangles[i].c);


        //    for (int j = 0; j < triangles[i].edges.Length; j++)
        //    {
        //        if (triangles[i].freeEdge[j] == false)
        //        {
        //            continue;
        //        }

        //        Vector3 midpoint = Vector3.Lerp(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1], 0.5f);
        //        point keyPoint = null;


        //        foreach (var item in pointsList)
        //        {

        //            if (item.pointCoord == triangles[i].a || item.pointCoord == triangles[i].b || item.pointCoord == triangles[i].c) { continue; }   //if the point is it self conntinue



        //            bool same = false;

        //            foreach (var tri in triangles)    // if this triangle already exissts
        //            {

        //                if (tri.HasVertex(triangles[i].edges[j].edge[0]) && tri.HasVertex(triangles[i].edges[j].edge[1]) && tri.HasVertex(item.pointCoord))
        //                {
        //                    same = true;

        //                    break;

        //                }
        //            }


        //            if (same) { break; }




        //            if (keyPoint == null)   // if this si the
        //            {
        //                keyPoint = item;
        //            }
        //            else
        //            {


        //                var dirKey = midpoint - keyPoint.pointCoord;
        //                var possibleDir = midpoint - item.pointCoord;



        //                if (Vector3.Angle(possibleDir, triPlane.normal) > Vector3.Angle(dirKey, triPlane.normal) && Vector3.Angle(possibleDir, triPlane.normal) < 180)
        //                {

        //                    keyPoint = item;
        //                }
        //            }
        //        }

        //        if (keyPoint != null)
        //        {


        //            var dirKeyg = midpoint - keyPoint.pointCoord;

        //            Debug.Log(Vector3.Angle(dirKeyg, triPlane.normal).ToString() + " as the accepted triangle");

        //            triangles.Add(new TriangleConvexHull(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1], keyPoint.pointCoord));

        //            triangles[triangles.Count - 1].EdgeToCancel(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1]);

        //        }

        //    }


        //}



    }



    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;

        //Gizmos.DrawSphere(points[0], 0.3f);
        //Gizmos.DrawSphere(points[1], 0.3f);


        //Gizmos.color = Color.black;
        //Gizmos.DrawSphere(points[2], 0.3f);


        //Plane triPlane = new Plane(points[0], points[1], points[2]);

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(points[3], 0.3f);





        //Vector3 midpoint = Vector3.Lerp(points[0], points[1], 0.5f);

        //Debug.Log(Vector3.Angle(points[3], triPlane.normal));



        foreach (var point in pointsList)
        {
            if (point.choosen)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(point.pointCoord, 0.3f);

        }







    }





    public class TriangleConvexHull
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Edge[] edges = new Edge[3];

        public bool[] freeEdge = new bool[3] { true, true, true };

        public void EdgeToCancel(Vector3 A, Vector3 B) 
        {
            for (int i = 0; i < edges.Length; i++)
            {
                if (edges[i].edge[0] == A  && edges[i].edge[1] == B   || edges[i].edge[0] == B && edges[i].edge[1] == A) 
                {
                    freeEdge[i] = false;
                }
            }
        }

        public TriangleConvexHull(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;


            this.edges[0] = new Edge(a, b);
            this.edges[1] = new Edge(b, c);
            this.edges[2] = new Edge(c, a);

        }




        // dont think this will be needed
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

    //check for the same shit 
    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }



    private void Update()
    {
        foreach (var triangle in triangles)
        {
            foreach (var edge in triangle.edges)
            {
                // Debug.Log($"{edge.edge[0][0]} and {edge.edge[0][1]}");
                Debug.DrawLine(new Vector3(edge.edge[0].x, edge.edge[0].y, edge.edge[0].z), new Vector3(edge.edge[1].x, edge.edge[1].y, edge.edge[1].z), Color.green);
            }
        }
    }


    [System.Serializable]
    public class point
    {
        public Vector3 pointCoord = Vector3.zero;
        public bool choosen = false;
    }




}
