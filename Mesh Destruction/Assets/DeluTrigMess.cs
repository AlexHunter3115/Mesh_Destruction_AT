using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeluTrigMess : MonoBehaviour
{

    public List<Vector3> vertices = new List<Vector3>();

    List<GeneralUtil.Triangle> triangles = new List<GeneralUtil.Triangle>();

    // Start is called before the first frame update
    void Start()
    {
        triangles = new List<GeneralUtil.Triangle>();

        //triangles = GeneralUtil.RunTriangulation(vertices);

        Debug.Log($"{triangles.Count}");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var tri in triangles)
        {
           // Debug.DrawLine(new Vector3(tri.edges[0].edge[0].x, tri.edges[0].edge[0].y, tri.edges[0].edge[0].z), new Vector3(tri.edges[0].edge[1].x, tri.edges[0].edge[1].y, tri.edges[0].edge[1].z), Color.green);
            //Debug.DrawLine(new Vector3(tri.edges[1].edge[0].x, tri.edges[1].edge[0].y, tri.edges[1].edge[0].z), new Vector3(tri.edges[1].edge[1].x, tri.edges[1].edge[1].y, tri.edges[1].edge[1].z), Color.green);
           // Debug.DrawLine(new Vector3(tri.edges[2].edge[0].x, tri.edges[2].edge[0].y, tri.edges[2].edge[0].z), new Vector3(tri.edges[2].edge[1].x, tri.edges[2].edge[1].y, tri.edges[2].edge[1].z), Color.green);
        }
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


    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }





    private void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        foreach (var vertex in vertices)
        {
            Gizmos.DrawSphere(vertex, 0.3f);
        }
    }
}
