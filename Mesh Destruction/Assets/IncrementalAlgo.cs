using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class IncrementalAlgo : MonoBehaviour
{


    public List<Vector3> points = new List<Vector3>();


    public Vector3 TetraA = new Vector3();
    public Vector3 TetraB = new Vector3();
    public Vector3 TetraC = new Vector3();
    public Vector3 TetraD = new Vector3();

    public List<tetraDeluTrig.Triangle> triangles = new List<tetraDeluTrig.Triangle>();

    // from what i understand its a while loop          for evey iter you have a list that is the    out    and the   in
    /*first all start in the out
     * 
     * pick a random 4 places to start and form the first conves
     * 
     * 
     * check which point is inside     take that point and move it to the in  the in array does  not get used its like a bin
     * 
     * go untill there is nothing in the out i do also think i need to put the vetrex in the out so they dont get selected as for the side whihc will becomes the new vertex idk
     * 
     * 
     * 
     * actually there are multiplle traingles  i think at the start of the iteration you then choose a random point to add to the overall convex     
     */

    // Start is called before the first frame update
    void Start()
    {




        //for (int i = 0; i < 30; i++)
        //{

        //    Vector3 point = new Vector3(Random.Range(1, 20), Random.Range(1, 20), Random.Range(1, 20));


        //    points.Add(point);

        //}


        TetraA = points[0];
        TetraB = points[1];
        TetraC = points[2];
        TetraD = points[3];






    }




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
            Gizmos.DrawSphere(points[i], 0.1f);
        }



    }






    void Update()
    {
        Debug.DrawLine(  TetraA   ,  TetraB  ,Color.green);
        Debug.DrawLine(  TetraB   ,  TetraD  ,Color.green);
        Debug.DrawLine(  TetraD   ,  TetraA  ,Color.green);

        Debug.DrawLine(  TetraB   ,  TetraC  ,Color.green);
        Debug.DrawLine(  TetraC   ,  TetraD  ,Color.green);

        Debug.DrawLine(TetraA, TetraC, Color.green);
        Debug.DrawLine(TetraC, TetraD, Color.green);

    }

}
