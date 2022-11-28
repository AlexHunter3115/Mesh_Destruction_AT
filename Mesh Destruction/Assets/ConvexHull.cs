
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;




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




    // the issue is that i am wokring with a plane and i am giving it a 3d angle



    // if you make a plane from the new point adn the two edges     shoooot a ray from the lone vertex to the mid point dir into the plane and get the ange



    [SerializeField]
    public List<Vector3> points = new List<Vector3>();
    public List<point> pointsList = new List<point>();

    [SerializeField]
    List<TriangleConvexHull> triangles = new List<TriangleConvexHull>();


    public bool test = false;
    public bool random = false;

    void Start()
    {
        if (!test)
        {



            if (random)
            {
                pointsList = new List<point>();

                for (int i = 0; i < 15; i++)
                {


                    pointsList.Add(new point());

                    pointsList[i].pointCoord = new Vector3(Random.Range(-4, 4), Random.Range(-4, 4), Random.Range(-4, 4));

                }

            }









            triangles = new List<TriangleConvexHull>();

            bool leave = false;

            //used to get the first triangle/plane  all works
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
            }


            // the only way to do this is to get the  



            //to many tris gets drawn


            for (int i = 0; i < triangles.Count; i++)  // for every triangle   triangles.Count
            {

                for (int j = 0; j < triangles[i].edges.Length; j++)   //check every edge
                {





                    //bool sameLine = false;

                    //int checkEdge = 0;

                    //for (int k = 0; k < triangles.Count; k++) //go through every triangle
                    //{


                    //    Debug.Log("-----------------------");
                    //    Debug.Log("-----------------------");
                    //    Debug.Log($"{i} {j} {k} ");
                    //    if (triangles[i].HasVertex(triangles[k].a) && triangles[i].HasVertex(triangles[k].b) && triangles[i].HasVertex(triangles[k].c))//if its not it self
                    //    {


                    //        //Debug.Log($"Same");

                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].a}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].b}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].c}");

                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[i].a}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[i].b}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[i].c}");

                    //        //Debug.Log(triangles[i].HasVertex(triangles[k].a));
                    //        //Debug.Log(triangles[i].HasVertex(triangles[k].b));
                    //        //Debug.Log(triangles[i].HasVertex(triangles[k].c));


                    //        Debug.Log($"Times called");
                    //    }
                    //    else
                    //    {




                    //        Debug.Log(triangles.Count);

                    //        //Debug.Log($"this si what the edge is right now  {triangles[i].edges[j].edge[0]}");
                    //        //Debug.Log($"this si what the edge is right now  {triangles[i].edges[j].edge[1]}");

                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].a}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].b}");
                    //        //Debug.Log($"this si what the trinagle is right now  {triangles[k].c}");



                    //        //check how many other edges of other trinangles match with this

                    //        if (triangles[k].HasVertex(triangles[i].edges[j].edge[0]) && triangles[k].HasVertex(triangles[i].edges[j].edge[1]))
                    //        {
                    //            checkEdge++;
                    //            //sameLine = true;
                                
                    //        }

                    //    }
                    //}
                    //if (sameLine == true)
                    //{
                    //    break;
                    //}











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


                    midpoint = new Vector3(midpoint.x, midpoint.y, midpoint.z);
                    point nextPoint = null;


                    foreach (var point in pointsList)  //go through all the points
                    {
                        if (point.pointCoord == triangles[i].a || point.pointCoord == triangles[i].b || point.pointCoord == triangles[i].c) { continue; }   //if the point is it self conntinue


                        var dirTri = alonevertex - midpoint;   // direction of the triangle

                        var dirNextPoint = point.pointCoord - midpoint; // direction of the new point to the edge

                        if (nextPoint == null)
                        {
                            nextPoint = point;
                        }
                        else
                        {
                            if (Vector3.Angle(dirTri, dirNextPoint) > Vector3.Angle(dirTri, nextPoint.pointCoord - midpoint)) { nextPoint = point; }   // if the angle is more that means its the extreme therefore add
                        }
                    }

                    bool exists = false;

                    foreach (var tri in triangles)  //check if the triangle already exists
                    {
                        if (tri.HasVertex(triangles[i].edges[j].edge[0]) && tri.HasVertex(triangles[i].edges[j].edge[1]) && tri.HasVertex(nextPoint.pointCoord))
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)  // if it doesnt exist add
                    {
                        triangles.Add(new TriangleConvexHull(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1], nextPoint.pointCoord));
                        //triangles[triangles.Count - 1].EdgeToCancel(triangles[i].edges[j].edge[0], triangles[i].edges[j].edge[1]);    //altought this is detected there is not actual change between this on or off

                        break;  // this seems to help a bit
                    }

                }
            }

        }
    }



    private void OnDrawGizmos()
    {




        if (!test)
        {

            for (int i = 0; i < pointsList.Count; i++)
            {

                if (pointsList[i].choosen)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(pointsList[i].pointCoord, 0.03f);


                Handles.Label(pointsList[i].pointCoord, i.ToString());

            }
        }
        else 
        {
            //for (int i = 0; i < 3; i++)
            //{
            //    if (i < 3) 
            //    {

            //        Gizmos.color = Color.green;
            //    }
            //    else 
            //    {
            //        Gizmos.color = Color.red;
            //    }



            //    Gizmos.DrawSphere(points[i], 0.1f);

            //}

            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(points[0], points[1]);
            Gizmos.DrawLine(points[1], points[2]);
            Gizmos.DrawLine(points[2], points[0]);


            Vector3 midpoint = Vector3.Lerp(points[1], points[0], 0.5f);



            Gizmos.color = Color.green;
            Gizmos.DrawSphere(midpoint, 0.03f);




            Gizmos.color = Color.blue;
            for (int i = 0; i < points.Count; i++)
            {

                var dirTri = points[2] - midpoint;   // alone vertex

                Gizmos.DrawSphere(points[i], 0.015f);
                Handles.Label(points[i], Vector3.Angle(dirTri, points[i] - midpoint).ToString() + "  " + i.ToString()) ;

            }

        }
    }





    public class TriangleConvexHull
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;

        public Edge[] edges = new Edge[3];


        //barely works
        //public void EdgeToCancel(Vector3 A, Vector3 B)
        //{
        //    for (int i = 0; i < edges.Length; i++)
        //    {
        //        if (edges[i].edge[0] == A && edges[i].edge[1] == B || edges[i].edge[0] == B && edges[i].edge[1] == A)
        //        {
        //            freeEdge[i] = false;
        //        }
        //    }
        //}

        public TriangleConvexHull(Vector3 a, Vector3 b, Vector3 c)
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



    //just for debugging 
    [System.Serializable]
    public class point
    {
        public Vector3 pointCoord = Vector3.zero;
        public bool choosen = false;
    }




}
