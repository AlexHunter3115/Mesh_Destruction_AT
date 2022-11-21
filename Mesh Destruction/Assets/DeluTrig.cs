using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeluTrig : MonoBehaviour
{
    [SerializeField]
    public List<Triangle> triangulation = new List<Triangle>();

    private Mesh mesh;

    public bool all;
    public bool draw;
    public bool call;

    private void Update()
    {
        if (all)
        {
            foreach (var triangle in triangulation)
            {
                foreach (var edge in triangle.edges)
                {
                    // Debug.Log($"{edge.edge[0][0]} and {edge.edge[0][1]}");
                    Debug.DrawLine(new Vector3(edge.edge[0][0], 0, edge.edge[0][1]), new Vector3(edge.edge[1][0], 0, edge.edge[1][1]), Color.green);
                }
            }
        }
        else
        {
            Debug.DrawLine(new Vector3(triangulation[0].edges[0].edge[0][0], 0, triangulation[0].edges[0].edge[0][1]), new Vector3(triangulation[0].edges[0].edge[1][0], 0, triangulation[0].edges[0].edge[1][1]), Color.green);
            Debug.DrawLine(new Vector3(triangulation[0].edges[1].edge[0][0], 0, triangulation[0].edges[1].edge[0][1]), new Vector3(triangulation[0].edges[1].edge[1][0], 0, triangulation[0].edges[1].edge[1][1]), Color.green);
            Debug.DrawLine(new Vector3(triangulation[0].edges[2].edge[0][0], 0, triangulation[0].edges[2].edge[0][1]), new Vector3(triangulation[0].edges[2].edge[1][0], 0, triangulation[0].edges[2].edge[1][1]), Color.green);


            Debug.DrawLine(new Vector3(triangulation[3].edges[0].edge[0][0], 0, triangulation[3].edges[0].edge[0][1]), new Vector3(triangulation[3].edges[0].edge[1][0], 0, triangulation[3].edges[0].edge[1][1]), Color.green);
            Debug.DrawLine(new Vector3(triangulation[3].edges[1].edge[0][0], 0, triangulation[3].edges[1].edge[0][1]), new Vector3(triangulation[3].edges[1].edge[1][0], 0, triangulation[3].edges[1].edge[1][1]), Color.green);
            Debug.DrawLine(new Vector3(triangulation[3].edges[2].edge[0][0], 0, triangulation[3].edges[2].edge[0][1]), new Vector3(triangulation[3].edges[2].edge[1][0], 0, triangulation[3].edges[2].edge[1][1]), Color.green);

        }




        if (call)
        {
            call = false;
            runtest();
        }


    }






    public void runtest()
    {



        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();


        //verticesList.Add(vertex)



        Vector3 p1a = new Vector3(triangulation[3].edges[2].edge[0][0], 0, triangulation[3].edges[2].edge[0][1]);
        Vector3 p2a = new Vector3(triangulation[3].edges[1].edge[0][0], 0, triangulation[3].edges[1].edge[0][1]);
        Vector3 p3a = new Vector3(triangulation[3].edges[0].edge[0][0], 0, triangulation[3].edges[0].edge[0][1]);
        Vector3 p1b = new Vector3(triangulation[0].edges[2].edge[0][0], 0, triangulation[0].edges[2].edge[0][1]);
        Vector3 p2b = new Vector3(triangulation[0].edges[1].edge[0][0], 0, triangulation[0].edges[1].edge[0][1]);
        Vector3 p3b = new Vector3(triangulation[0].edges[0].edge[0][0], 0, triangulation[0].edges[0].edge[0][1]);






        verticesList.Add(p1a);
        verticesList.Add(p2a);
        verticesList.Add(p3a);


        verticesList.Add(p1b);
        verticesList.Add(p2b);
        verticesList.Add(p3b);



        foreach (var item in verticesList)
        {
            Debug.Log(item);
            Instantiate(MeshGenerator.Instance.placeHolders, item, this.transform.rotation);
        }




        trianglesList.Add(0);
        trianglesList.Add(1);
        trianglesList.Add(2);



        trianglesList.Add(3);
        trianglesList.Add(4);
        trianglesList.Add(5);


        mesh.vertices = verticesList.ToArray();


        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
        this.transform.GetComponent<MeshRenderer>().material = MeshGenerator.Instance.material;
    }


    private void OnDrawGizmos()
    {
        if (draw)
        {

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(new Vector3(triangulation[3].edges[2].edge[0][0], 0, triangulation[3].edges[2].edge[0][1]), 1);
            Gizmos.DrawSphere(new Vector3(triangulation[3].edges[1].edge[0][0], 0, triangulation[3].edges[1].edge[0][1]), 1);
            Gizmos.DrawSphere(new Vector3(triangulation[3].edges[0].edge[0][0], 0, triangulation[3].edges[0].edge[0][1]), 1);
            Gizmos.DrawSphere(new Vector3(triangulation[0].edges[2].edge[0][0], 0, triangulation[0].edges[2].edge[0][1]), 1);
            Gizmos.DrawSphere(new Vector3(triangulation[0].edges[1].edge[0][0], 0, triangulation[0].edges[1].edge[0][1]), 1);
            Gizmos.DrawSphere(new Vector3(triangulation[0].edges[0].edge[0][0], 0, triangulation[0].edges[0].edge[0][1]), 1);

        }
    }


    private void Start()
    {

        mesh = this.transform.GetComponent<MeshFilter>().mesh;

        List<Vector2> pointList = new List<Vector2>();


        for (int i = 0; i < 50; i++)
        {

            Vector2 point = new Vector2(Random.Range(1, 20), Random.Range(1, 20));


            if (pointList.Contains(point))
            {
            }
            else { pointList.Add(point); }

        }

        triangulation = new List<Triangle>();

        Vector2 superTriangleA = new Vector2(10000, 10000);
        Vector2 superTriangleB = new Vector2(10000, 0);
        Vector2 superTriangleC = new Vector2(0, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));


        foreach (Vector2 point in pointList)
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
                triangulation.Remove(badTriangle);
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




    }


    public class Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;

        public Edge[] edges = new Edge[3];

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;


            this.edges[0] = new Edge(a, b);
            this.edges[1] = new Edge(b, c);
            this.edges[2] = new Edge(c, a);
        }

        public bool HasVertex(Vector2 point)
        {
            if (a == point || b == point || c == point) { return true; }
            else { return false; }
        }

    }

    public class Edge
    {
        public Vector2[] edge = new Vector2[2];

        public Edge(Vector2 a, Vector2 b)
        {
            edge[0] = a;
            edge[1] = b;
        }
    }


    /// <summary>
    /// returns true if the point is in the circle
    /// </summary>
    public bool IspointInCircumcircle(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
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

    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }
}
