using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System.Linq;

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


    public bool test = false;
    
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






        //basis of algo

        //foreach (var tri in triangles)
        //{
        //    Plane plane = new Plane(tri.a, tri.b, tri.c);


        //    if (plane.GetSide(points[8])) 
        //    {
        //        Debug.Log("positive");
        //    }
        //    else 
        //    {
        //        Debug.Log("negative");
        //    }

        //}

        // could prob cheat my wait out and use it like it is right now and do a check if the tri has no getside so we know which oneare facing out



        int randomIndex = 15;
        //we could be very not efficient and redo the whole list avery time







        List<int> interestedTris = new List<int>();




        for (int i = 0; i < triangles.Count; i++)
        {
            Plane plane = new Plane(triangles[i].a, triangles[i].b, triangles[i].c);


            if (plane.GetSide(points[randomIndex]))
            {

                interestedTris.Add(i);

                //tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

                //newTris[0] = (new tetraDeluTrig.Triangle(triangles[i].edges[0].edge[0], triangles[i].edges[0].edge[1], points[randomIndex]));
                //newTris[1] = (new tetraDeluTrig.Triangle(triangles[i].edges[1].edge[0], triangles[i].edges[1].edge[1], points[randomIndex]));
                //newTris[2] = (new tetraDeluTrig.Triangle(triangles[i].edges[2].edge[0], triangles[i].edges[2].edge[1], points[randomIndex]));

                //triangles.RemoveAt(i);  //should remove that tri


                //foreach (var newTri in newTris)
                //{
                //    triangles.Add(newTri);
                //}
                //break;

            }

        }


        interestedTris.Sort();
        interestedTris.Reverse();


        //foreach (var indexTri in interestedTris)
        //{


        //    tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

        //    newTris[0] = (new tetraDeluTrig.Triangle(triangles[indexTri].edges[0].edge[0], triangles[indexTri].edges[0].edge[1], points[randomIndex]));
        //    newTris[1] = (new tetraDeluTrig.Triangle(triangles[indexTri].edges[1].edge[0], triangles[indexTri].edges[1].edge[1], points[randomIndex]));
        //    newTris[2] = (new tetraDeluTrig.Triangle(triangles[indexTri].edges[2].edge[0], triangles[indexTri].edges[2].edge[1], points[randomIndex]));

        //    //should remove that tri


        //    foreach (var newTri in newTris)
        //    {
        //        triangles.Add(newTri);
        //    }


        //}



        List<tetraDeluTrig.Edge> interestedEdges = new List<tetraDeluTrig.Edge>();
        List<tetraDeluTrig.Triangle> rejectedTrigs = new List<tetraDeluTrig.Triangle>();

        Debug.Log(triangles.Count);
        foreach (var removeIdx in interestedTris)
        {
            rejectedTrigs.Add(triangles[removeIdx]);
            triangles.RemoveAt(removeIdx);
        }



        Debug.Log(interestedEdges.Count);



        ////Debug.Log(triangles.Count);

        //interestedTris = new List<int>();


        //int numTris = triangles.Count;


        // might need to add a if there is more than one rejected trig

        for (int i = 0; i < rejectedTrigs.Count; i++)
        {

            for (int j = 0; j < rejectedTrigs.Count; j++)
            {
                if (i == j) 
                {
                    continue; 
                }

                foreach (var edgePrime in rejectedTrigs[i].edges)
                { // for each edge in the main trig

                    bool add = true;

                    foreach (var edgeSecond in rejectedTrigs[j].edges)
                    {
                        if (LineIsEqual(edgePrime, edgeSecond)) 
                        { 
                            add = false; 
                            break;
                        }
                    }


                    if (add) 
                    {
                        bool actuallyAdd = true;

                        foreach (var addedEdge in interestedEdges)
                        {
                            if (LineIsEqual(edgePrime, addedEdge))
                            {
                                actuallyAdd = false;
                                break;
                            }
                        }

                        if (actuallyAdd) 
                        {
                            interestedEdges.Add(edgePrime);
                        }
                    }

                }
            }


        }




        Debug.Log(interestedEdges.Count);




        foreach (var edges in interestedEdges)
        {
            triangles.Add(new tetraDeluTrig.Triangle(edges.edge[0], edges.edge[1], points[randomIndex]));
        }

        //need more testing but for now i am done













        /*


        add all of the edges 









        //for (int i = triangles.Count; i-- > 0;)
        //{
        //    int similarEdge = 0;


        //    for (int j = triangles.Count; j-- > 0;)
        //    {
        //        if (i == j)
        //        {
        //            //same tri nothing here
        //        }
        //        else
        //        {
        //            foreach (var edgePrime in triangles[i].edges)
        //            {
        //                //Debug.Log("---------------------");
        //                //Debug.Log("---------------------");
        //                //Debug.Log(triangles.Count);
        //                //Debug.Log(i);
        //                //Debug.Log(j);
        //                foreach (var edgeSecond in triangles[j].edges)
        //                {
        //                    if (LineIsEqual(edgePrime, edgeSecond)) { similarEdge++; }
        //                }

        //            }
        //        }


        //        if (similarEdge > 4)
        //        {

        //            triangles.RemoveAt(i);
        //            break;
        //        }

        //    }
        //    //do something
        //}





        // in the vid the edges are outliers they ar eno shared with eachother of the taken out trinagles










        //for (int i = 0; i < numTris; i++)
        //{
        //    int similarEdge = 0;


        //    for (int j = 0; j < numTris; j++)
        //    {
        //        if (i == j)
        //        {
        //            //same tri nothing here
        //        }
        //        else
        //        {
        //            foreach (var edgePrime in triangles[i].edges)
        //            {

        //                foreach (var edgeSecond in triangles[j].edges)
        //                {
        //                    if (LineIsEqual(edgePrime, edgeSecond)) { similarEdge++; }
        //                }

        //            }
        //        }


        //        if (similarEdge > 4)
        //        {
        //            //interestedTris.Add(i);
                    
        //            triangles.RemoveAt(i);
        //            numTris  =triangles.Count;
        //            break;
        //        }





        //    }


        //}









        //Debug.Log(triangles.Count);

        //interestedTris.Sort();
        //interestedTris.Reverse();


        //foreach (var removeIdx in interestedTris)
        //{
        //    Debug.Log(removeIdx);
        //    triangles.RemoveAt(removeIdx);
        //}



        //Debug.Log(triangles.Count);






        //List<tetraDeluTrig.Triangle> copyTriList = new List<tetraDeluTrig.Triangle>();



        //foreach (var tri in triangles) 
        //{
        //    copyTriList.Add(tri);
        //}



        //for (int i = 0; i < copyTriList.Count; i++)
        //{
        //    Plane plane = new Plane(copyTriList[i].a, copyTriList[i].b, copyTriList[i].c);


        //    if (plane.GetSide(points[randomIndex]))
        //    {
        //        Debug.Log("positive");


        //        tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

        //        newTris[0] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[0].edge[0], copyTriList[i].edges[0].edge[1], points[randomIndex]));
        //        newTris[1] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[1].edge[0], copyTriList[i].edges[1].edge[1], points[randomIndex]));
        //        newTris[2] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[2].edge[0], copyTriList[i].edges[2].edge[1], points[randomIndex]));

        //        triangles.RemoveAt(i);  //should remove that tri


        //        foreach (var newTri in newTris)
        //        {
        //            triangles.Add(newTri);
        //        }
        //        break;

        //    }

        //}





        //randomIndex = 15;




        //copyTriList = new List<tetraDeluTrig.Triangle>();


        //foreach (var tri in triangles)
        //{
        //    copyTriList.Add(tri);
        //}


        //for (int i = 0; i < copyTriList.Count; i++)
        //{
        //    Plane plane = new Plane(copyTriList[i].a, copyTriList[i].b, copyTriList[i].c);


        //    if (plane.GetSide(points[randomIndex]))
        //    {
        //        Debug.Log("positive");


        //        tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

        //        newTris[0] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[0].edge[0], copyTriList[i].edges[0].edge[1], points[randomIndex]));
        //        newTris[1] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[1].edge[0], copyTriList[i].edges[1].edge[1], points[randomIndex]));
        //        newTris[2] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[2].edge[0], copyTriList[i].edges[2].edge[1], points[randomIndex]));

        //        triangles.RemoveAt(i);  //should remove that tri


        //        foreach (var newTri in newTris)
        //        {
        //            triangles.Add(newTri);
        //        }


        //    }

        //}









        //randomIndex = 7;

        //copyTriList = new List<tetraDeluTrig.Triangle>();


        //foreach (var tri in triangles)
        //{
        //    copyTriList.Add(tri);
        //}


        //for (int i = 0; i < copyTriList.Count; i++)
        //{
        //    Plane plane = new Plane(copyTriList[i].a, copyTriList[i].b, copyTriList[i].c);


        //    if (plane.GetSide(points[randomIndex]))
        //    {
        //        Debug.Log("positive");


        //        tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

        //        newTris[0] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[0].edge[0], copyTriList[i].edges[0].edge[1], points[randomIndex]));
        //        newTris[1] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[1].edge[0], copyTriList[i].edges[1].edge[1], points[randomIndex]));
        //        newTris[2] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[2].edge[0], copyTriList[i].edges[2].edge[1], points[randomIndex]));

        //        triangles.RemoveAt(i);  //should remove that tri


        //        foreach (var newTri in newTris)
        //        {
        //            triangles.Add(newTri);
        //        }


        //    }

        //}



        //randomIndex = 7;


        //copyTriList = new List<tetraDeluTrig.Triangle>();

        //copyTriList = triangles;


        //for (int i = 0; i < copyTriList.Count; i++)
        //{
        //    Plane plane = new Plane(copyTriList[i].a, copyTriList[i].b, copyTriList[i].c);


        //    if (plane.GetSide(points[randomIndex]))
        //    {
        //        Debug.Log("positive");


        //        tetraDeluTrig.Triangle[] newTris = new tetraDeluTrig.Triangle[3];

        //        newTris[0] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[0].edge[0], copyTriList[i].edges[0].edge[1], points[randomIndex]));
        //        newTris[1] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[1].edge[0], copyTriList[i].edges[1].edge[1], points[randomIndex]));
        //        newTris[2] = (new tetraDeluTrig.Triangle(copyTriList[i].edges[2].edge[0], copyTriList[i].edges[2].edge[1], points[randomIndex]));

        //        triangles.RemoveAt(i);  //should remove that tri


        //        foreach (var newTri in newTris)
        //        {
        //            triangles.Add(newTri);
        //        }


        //    }

        //}

        */




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







    }






    private List<tetraDeluTrig.Triangle> NewTetra(tetraDeluTrig.Triangle triToChange, Vector3 newPoint) 
    {
        List<tetraDeluTrig.Triangle> newTris = new List<tetraDeluTrig.Triangle>();

        newTris.Add(new tetraDeluTrig.Triangle(triToChange.edges[0].edge[0], triToChange.edges[0].edge[1], newPoint));
        newTris.Add(new tetraDeluTrig.Triangle(triToChange.edges[1].edge[0], triToChange.edges[1].edge[1], newPoint));
        newTris.Add(new tetraDeluTrig.Triangle(triToChange.edges[2].edge[0], triToChange.edges[2].edge[1], newPoint));

        return newTris;
        
    }


    // to check
    public bool LineIsEqual(tetraDeluTrig.Edge A, tetraDeluTrig.Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }




    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;

        ////foreach (var point in points)
        ////{
        ////    Gizmos.DrawSphere(point, 0.1f);
        ////}
        //Gizmos.DrawSphere(points[0], 0.1f);
        //Gizmos.DrawSphere(points[1], 0.1f);
        //Gizmos.DrawSphere(points[2], 0.1f);
        //Gizmos.DrawSphere(points[3], 0.1f);


        for (int i = 0; i < points.Count; i++)
        {

            if (i < 4)
            {
                Gizmos.color = Color.red;
            }
            else 
            {

                Gizmos.color = Color.green;
            }


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
