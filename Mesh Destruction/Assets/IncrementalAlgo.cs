using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class IncrementalAlgo : MonoBehaviour
{


    public List<Vector3> points = new List<Vector3>();


    public Vector3 savedPos = new Vector3();
    public List<tetraDeluTrig.Triangle> triangles = new List<tetraDeluTrig.Triangle>();

    // from what i understand its a while loop          for evey iter you have a list that is the    out    and the   in
    /*first all start in the out
     * 
     * pick a random 4 places to start and form the first conves
     * 
     * 
     * check which point is inside     take that point and move it to the in  the in array does  not get used its like a bin
     * fuck convex hull pirce of shit
     * go untill there is nothing in the out i do also think i need to put the vetrex in the out so they dont get selected as for the side whihc will becomes the new vertex idk
     * 
     * 
     * 
     * actually there are multiplle traingles  i think at the start of the iteration you then choose a random point to add to the overall convex     
     */

    
    void Start()
    {


        List<int> triangleIndex = new List<int>();

        List<Vector3> vertex = new List<Vector3>();


        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();


        triangles.Add(new tetraDeluTrig.Triangle(points[0], points[1], points[2]));
        triangles.Add(new tetraDeluTrig.Triangle(points[3], points[2], points[1]));
        triangles.Add(new tetraDeluTrig.Triangle(points[0], points[2], points[3]));
        triangles.Add(new tetraDeluTrig.Triangle(points[1], points[0], points[3]));


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

        



        foreach (var point in points)
        {















            /*

            foreach (var tri in triangles)
            {
                Plane plane = new Plane(tri.a, tri.b, tri.c);

                if (plane.GetSide(points[8]))
                {
                    Debug.Log($"This is on the tru side {tri.a} {tri.b} {tri.c}");
                }
                else
                {
                    // this is on the inside apparently
                    savedPos = point;
                    Debug.Log($"this is on the false side {tri.a} {tri.b} {tri.c}");
                }
            }

            */
        }


    













        foreach (var point in points)
        {

            bool inside = false;
     


            if (inside) 
            {
                savedPos = point;
            }


        }







    }



    //its not fucking tetra is the whole convex fucking hull
    
    /*
    public bool IsInsideTetra(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Vector3 E) 
    {
        Plane tri1 = new Plane(A, B, D);
        Plane tri2 = new Plane(A, C, D);
        Plane tri3 = new Plane(B, C, D);
        Plane tri4 = new Plane(A, C, B);




        if (tri1.GetSide(E) && tri2.GetSide(E) && tri3.GetSide(E) && tri4.GetSide(E))
        {

            return true;


        }
        else

            return false;




    
    }

    */



    



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //foreach (var point in points)
        //{
        //    Gizmos.DrawSphere(point, 0.1f);
        //}
        Gizmos.DrawSphere(points[0], 0.1f);
        Gizmos.DrawSphere(points[1], 0.1f);
        Gizmos.DrawSphere(points[2], 0.1f);
        Gizmos.DrawSphere(points[3], 0.1f);


        Gizmos.color = Color.green;
        for (int i = 4; i < points.Count; i++)
        {
            Handles.Label(points[i], i.ToString());
            Gizmos.DrawSphere(points[i], 0.1f);
        }


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(savedPos, 0.7f);

    }






    void Update()
    {
        foreach (var tri in triangles)
        {//can use the .a thing
            foreach (var edge in tri.edges)
            {
                Debug.DrawLine(edge.edge[0], edge.edge[1], Color.red);
            }
        }
    }

}
