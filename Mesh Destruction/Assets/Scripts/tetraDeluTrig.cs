
using System.Collections.Generic;
using UnityEngine;
using static tetraDeluTrig;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class tetraDeluTrig : MonoBehaviour
{
    // this class was taken from my own Dissertation project



    [SerializeField]
    public List<Tetrahederal> Tetraingulation = new List<Tetrahederal>();

    public List<Vector3> pointsTest = new List<Vector3>();

    [SerializeField]
    private List<Vector3> pointList = new List<Vector3>();


    public Vector3 testTetra = Vector3.zero;


    public Material mat;

    List<Edge> Edges = new List<Edge>();
    List<Triangle> Triangles = new List<Triangle>();


    public bool newRanPoints = false;
    [SerializeField]
    private enum State 
    {
        DRAWTETRATEST,
        DRAWPOINTS,
        DRAWPOINTSTEST,
        DRAWTETRA
    
    }
    [SerializeField]
    private State state;


    public Vector3 TetraTestA = new Vector3(20, 10, 0);
    public Vector3 TetraTestB = new Vector3(10, 30, 0);
    public Vector3 TetraTestC = new Vector3(13, 0, 20);
    public Vector3 TetraTestD = new Vector3(14, 23, 6);




    public Vector3 centreOfPoints = new Vector3(0, 0, 0);



    private void OnDrawGizmos()
    {
        switch (state)
        {
            case State.DRAWTETRATEST:

                Tetrahederal tetra = new Tetrahederal(TetraTestA, TetraTestB, TetraTestC, TetraTestD);

                Gizmos.color = Color.red;

                Gizmos.DrawSphere(tetra.A, 2f);
                Gizmos.DrawSphere(tetra.B, 2f);
                Gizmos.DrawSphere(tetra.C, 2f);
                Gizmos.DrawSphere(tetra.D, 2f);


                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(tetra.CircumSphereCenter, 2);
                Gizmos.DrawWireSphere(tetra.CircumSphereCenter, tetra.CircumSphereRadius);




                if (GeneralUtil.IspointInCircumsphere(tetra.A, tetra.B, tetra.C, tetra.D, testTetra)) 
                {
                    Gizmos.color = Color.green;
                }
                else 
                {
                    Gizmos.color = Color.red;
                }
                Gizmos.DrawSphere(testTetra, 2f);



                break;
            case State.DRAWPOINTS:


                foreach (var point in pointList)
                {
                    Gizmos.DrawSphere(point, 1f);
                }
                Gizmos.color = Color.red;

                Gizmos.DrawSphere(avaragePoints(pointList), 2f);


                break;

            case State.DRAWTETRA:

                foreach (var edge in Edges)
                {
                    Gizmos.DrawLine(edge.edge[0], edge.edge[1]);
                }


                Gizmos.color = Color.red;

                Gizmos.DrawSphere(avaragePoints(pointList), 2f);


                break;


            case State.DRAWPOINTSTEST:


                foreach (var point in pointsTest)
                {
                    Gizmos.DrawSphere(point, 1f);
                }
                Gizmos.color = Color.red;

                Gizmos.DrawSphere(avaragePoints(pointsTest), 2f);


                break;

            default:
                break;
        }

    }






    private void Start()
    {


        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        //pointList = new List<Vector3>();


        //for (int i = 0; i < 30; i++)
        //{

        //    Vector3 point = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40, 40));


        //    pointList.Add(point);

        //}


        centreOfPoints = avaragePoints(pointsTest);


        Tetraingulation = new List<Tetrahederal>();

        //Vector3 superTriangleA = new Vector3(10000, 10000, 0);
        //Vector3 superTriangleB = new Vector3(10000, 0, 10000);
        //Vector3 superTriangleC = new Vector3(0, 10000, 0);
        //Vector3 superTriangleD = new Vector3(0, 10000, 10000);


        //Tetraingulation.Add(new Tetrahederal(superTriangleA, superTriangleB, superTriangleC, superTriangleD));




        float minX = pointsTest[0].x;
        float minY = pointsTest[0].y;
        float minZ = pointsTest[0].z;
        float maxX = minX;
        float maxY = minY;
        float maxZ = minZ;

        foreach (var vertex in pointsTest)
        {
            if (vertex.x < minX) minX = vertex.x;
            if (vertex.x > maxX) maxX = vertex.x;
            if (vertex.y < minY) minY = vertex.y;
            if (vertex.y > maxY) maxY = vertex.y;
            if (vertex.z < minZ) minZ = vertex.z;
            if (vertex.z > maxZ) maxZ = vertex.z;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float dz = maxZ - minZ;
        float deltaMax = Mathf.Max(dx, dy, dz) * 2;

        Vector3 p1 = new Vector3(minX - 1, minY - 1, minZ - 1);
        Vector3 p2 = new Vector3(maxX + deltaMax, minY - 1, minZ - 1);
        Vector3 p3 = new Vector3(minX - 1, maxY + deltaMax, minZ - 1);
        Vector3 p4 = new Vector3(minX - 1, minY - 1, maxZ + deltaMax);

        Tetraingulation.Add(new Tetrahederal(p1, p2, p3, p4));








        foreach (var point in pointsTest) 
        {
            List<Triangle> triangles = new List<Triangle>();

            foreach (var tetra in Tetraingulation)
            {
                if (GeneralUtil.IspointInCircumsphere(tetra.A, tetra.B, tetra.C, tetra.D, point)) 
                {
                    tetra.isBad = true;

                    triangles.Add(new Triangle(tetra.A, tetra.B, tetra.C));
                    triangles.Add(new Triangle(tetra.A, tetra.B, tetra.D));
                    triangles.Add(new Triangle(tetra.A, tetra.C, tetra.D));
                    triangles.Add(new Triangle(tetra.B, tetra.C, tetra.D));

                }
            }




            for (int i = 0; i < triangles.Count; i++)
            {

                for (int j = i + 1; j < triangles.Count; j++)
                {

                    if (AlmostEqual(triangles[i], triangles[j]))
                    {

                        triangles[i].isBad = true;
                        triangles[j].isBad = true;
                    }
                }
            }


            Tetraingulation.RemoveAll((Tetrahederal t) => t.isBad);
            triangles.RemoveAll((Triangle t) => t.isBad);

            foreach (var triangle in triangles)
            {
                Tetraingulation.Add(new Tetrahederal(triangle.a, triangle.b, triangle.c, point));
            }


        }


        Tetraingulation.RemoveAll((Tetrahederal t) => t.HasVertex(p1) || t.HasVertex(p2) || t.HasVertex(p3) || t.HasVertex(p4));


        HashSet<Triangle> triangleSet = new HashSet<Triangle>();
        HashSet<Edge> edgeSet = new HashSet<Edge>();


        foreach (var t in Tetraingulation)
        {
            var abc = new Triangle(t.A, t.B, t.C);
            var abd = new Triangle(t.A, t.B, t.D);
            var acd = new Triangle(t.A, t.C, t.D);
            var bcd = new Triangle(t.B, t.C, t.D);

            if (triangleSet.Add(abc))
            {
                Triangles.Add(abc);
            }

            if (triangleSet.Add(abd))
            {
                Triangles.Add(abd);
            }

            if (triangleSet.Add(acd))
            {
                Triangles.Add(acd);
            }

            if (triangleSet.Add(bcd))
            {
                Triangles.Add(bcd);
            }

            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);
            var da = new Edge(t.D, t.A);
            var db = new Edge(t.D, t.B);
            var dc = new Edge(t.D, t.C);

            if (edgeSet.Add(ab))
            {
                Edges.Add(ab);
            }

            if (edgeSet.Add(bc))
            {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca))
            {
                Edges.Add(ca);
            }

            if (edgeSet.Add(da))
            {
                Edges.Add(da);
            }

            if (edgeSet.Add(db))
            {
                Edges.Add(db);
            }

            if (edgeSet.Add(dc))
            {
                Edges.Add(dc);
            }
        }








        //need a way to get all fo the outside points


        List<int> triangleIndex = new List<int>();
        List<Vector3> vertice = new List<Vector3>();




        foreach (var tri in Triangles)
        {
            tri.sortedList.Add(tri.a);
            tri.sortedList.Add(tri.b);
            tri.sortedList.Add(tri.c);

            Vector3 dir = centreOfPoints - avaragePoints(tri.sortedList);

            tri.sortedList = sortVerticies(dir, tri.sortedList);
            tri.sortedList.Reverse();

            bool draw = true;

            Plane plane = new Plane(tri.sortedList[0], tri.sortedList[1], tri.sortedList[2]);

            foreach (var p in pointsTest)
            {
                if (p != tri.a || p != tri.b || p != tri.c)
                {
                    if (plane.GetSide(p))
                    {
                        draw = false;
                        break;
                    }
                }
            }



            //if (plane.GetSide(centreOfPoints)) 
            //{
            //    draw = false; 
            //}



            if (draw) 
            {
                foreach (var sortedPos in tri.sortedList)
                {
                    triangleIndex.Add(vertice.Count);
                    vertice.Add(sortedPos);
                }
            }
                
            
        }




        //foreach (var tri in Triangles)
        //{
        //    bool drawTrig = true;


        //    foreach (var edge in tri.edges)
        //    {

        //        foreach (var otherTri in Triangles)
        //        {


        //            if (tri == otherTri) { continue; }

        //            int equalEdges = 0;

        //            foreach (var otherEdge in otherTri.edges)
        //            {


        //                if (LineIsEqual(otherEdge, edge)) 
        //                {
        //                    equalEdges++;
        //                }


        //            }


        //            if (equalEdges > 6) 
        //            {
        //                drawTrig = false;
        //            }



        //        }


        //    }


        //    if (drawTrig) 
        //    {
        //        triangleIndex.Add(vertice.Count);
        //        vertice.Add(tri.a);
        //        triangleIndex.Add(vertice.Count);
        //        vertice.Add(tri.b);
        //        triangleIndex.Add(vertice.Count);
        //        vertice.Add(tri.c);
        //    }

        //}










        //foreach (var tri in Triangles)
        //{

        //    triangleIndex.Add(vertice.Count);
        //    vertice.Add(tri.a);
        //    triangleIndex.Add(vertice.Count);
        //    vertice.Add(tri.b);
        //    triangleIndex.Add(vertice.Count);
        //    vertice.Add(tri.c);
        //}


        mesh.vertices = vertice.ToArray();
        mesh.triangles = triangleIndex.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        this.GetComponent<MeshRenderer>().material = mat;



    }









    public List<Vector3> sortVerticies(Vector3 normal, List<Vector3> nodes)
    {


        Vector3 first = nodes[0];

        //Sort by distance from random point to get 2 adjacent points.
        List<Vector3> temp = nodes.OrderBy(n => Vector3.Distance(n, first)).ToList();

        //Create a vector from the 2 adjacent points,
        //this will be used to sort all points, except the first, by the angle to this vector.
        //Since the shape is convex, angle will not exceed 180 degrees, resulting in a proper sort.
        Vector3 refrenceVec = (temp[1] - first);

        //Sort by angle to reference, but we are still missing the first one.
        List<Vector3> results = temp.Skip(1).OrderBy(n => Vector3.Angle(refrenceVec, n - first)).ToList();

        //insert the first one, at index 0.
        results.Insert(0, nodes[0]);

        //Now that it is sorted, we check if we got the direction right, if we didn't we reverse the list.
        //We compare the given normal and the cross product of the first 3 point.
        //If the magnitude of the sum of the normal and cross product is less than Sqrt(2) then then there is more than 90 between them.
        if ((Vector3.Cross(results[1] - results[0], results[2] - results[0]).normalized + normal.normalized).magnitude < 1.414f)
        {
            results.Reverse();
        }

        return results;
    }






    public Vector3 avaragePoints(List<Vector3> pointss) 
    {

        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        foreach (var point in pointss)
        {
            totalX += point.x;
            totalY += point.y;
            totalZ += point.z;
        }

        centreOfPoints = new Vector3(totalX / pointList.Count, totalY / pointList.Count, totalZ / pointList.Count);

        return centreOfPoints;

    }








    public static bool AlmostEqual(Triangle left, Triangle right)
    {
        return (AlmostEqual(left.a, right.a) ||AlmostEqual(left.a, right.c) ||AlmostEqual(left.a, right.c))
            && (AlmostEqual(left.b, right.a) || AlmostEqual(left.b, right.b) || AlmostEqual(left.b, right.c))
            && (AlmostEqual(left.c, right.b) ||AlmostEqual(left.c, right.b) || AlmostEqual(left.c, right.c));
    }



    // interesting 
    static bool AlmostEqual(Vector3 left, Vector3 right)
    {
        return (left - right).sqrMagnitude < 0.01f;
    }




    private void Update()
    {



        if (newRanPoints) 
        {

            newRanPoints = false;

            pointList.Clear();

            for (int i = 0; i < 30; i++)
            {

                pointList.Add(new Vector3(Random.Range(-20, 20), Random.Range(-20, 20), Random.Range(-20, 20)));

            }


        }

    }



    
    public class Tetrahederal 
    {


        public bool isBad = false;

        public Triangle[] triangles = new Triangle[4];


        public Vector3 A = new Vector3();
        public Vector3 B = new Vector3();
        public Vector3 C = new Vector3();
        public Vector3 D = new Vector3();

        public Vector3 CircumSphereCenter = Vector3.zero;
        public float CircumSphereRadius = 0;
        public float CircumSphereDiam = 0;

        public Tetrahederal (Vector3 A, Vector3 B, Vector3 C, Vector3 D) 
        {

            this.A = A;
            this.D = D;
            this.B = B;
            this.C = C;

            triangles[0] = new Triangle(A, B, C);
            triangles[1] = new Triangle(A, B, D);
            triangles[2] = new Triangle(C, B, D);
            triangles[3] = new Triangle(C, A, D);

            CircumSphereCenter = GeneralUtil.Circumcentre(A,B,C,D);



             CircumSphereRadius = Vector3.Distance(this.CircumSphereCenter, A);
            CircumSphereDiam = CircumSphereRadius* 2;
        }


        public bool HasVertex(Vector3 point)
        {
            if (point == A || point == B || point == C || point == D) { return true; }
            else { return false; }
        }







        #region toDelete


        #endregion
















    }

    public class Triangle
    {

        public List<Vector3> sortedList = new List<Vector3>();

        public bool isBad = false;

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


   
    // to check
    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }





}
