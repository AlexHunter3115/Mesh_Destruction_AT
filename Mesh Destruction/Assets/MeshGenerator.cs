using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using static note;
using static UnityEngine.UI.Image;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    public static MeshGenerator Instance;


    Mesh mesh;

    public Material material;


    List<Vector3> vertices = new List<Vector3>();
    int[] triangles = new int[]
    {
       0,2,1,   // F    works
        1,2,3,

        1,3,7,   //R    works
        7,3,5,

        6,4,0,    //L   works
        0,4,2,

        7,5,6,     //Ba
        6,5,4,

        2,4,3,   // T  works
        3,4,5,

        6,0,7,     //bot    works
        7,0,1

    };




    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();




    public GameObject planeObj;

    public GameObject placeHolders;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mesh = this.transform.GetComponent<MeshFilter>().mesh;


    }



    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.D)) 
    //    {

    //        CallHalving();
    //    }   
    //}





    //public void BuildReplica(List<Vector3> _vertices, int[] tris) 
    //{

    //    mesh = new Mesh();

    //    GetComponent<MeshFilter>().mesh = mesh;


    //    vertices = _vertices;



    //    for (int i = 0; i < _vertices.Count; i++)
    //    {
    //       // Debug.Log($"{_vertices[i]}");
    //    }



    //    //UpdateMesh(tris);
    //}


    //public void BuildReplica(Vector3[] _vertices, int[] tris)
    //{

    //    mesh = new Mesh();

    //    GetComponent<MeshFilter>().mesh = mesh;




    //    //UpdateMesh(tris,_vertices);
    //}


    //public void CreatePlane() 
    //{
    //    vertices = new Vector3[]
    //    {
    //        new Vector3 (0,2,0),
    //        new Vector3 (0,2,1),
    //        new Vector3 (1,2,0),
    //        new Vector3 (1,2,1)
    //    };

    //    triangles = new int[]
    //    {
    //        0,1,2,
    //        1,3,2
    //    };

    //}


    //public void UpdateMesh(int[] tris, Vector3[] vert)
    //{

    //   // mesh.Clear();

    //    List<Vector3> vertices = new List<Vector3>();
    //    List<int> triangles = new List<int>();

    //    int triesToDraw = 6;


    //    if (triesToDraw > mesh.triangles.Length / 3)
    //    {
    //        triesToDraw = triesToDraw / 3;
    //    }
    //    else if (triesToDraw <= 0)
    //    {
    //        triesToDraw = 1;
    //    }


    //    //Debug.Log($"{mesh.triangles.Length}");    //36  divided by 3 is 12   which is the number of tris

    //    for (int i = 0; i < mesh.triangles.Length; i += 3)
    //    {
    //        string stri = string.Empty;


    //        Vector3 p1 = mesh.vertices[mesh.triangles[i + 0]];
    //        Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
    //        Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];

    //        vertices.Add(p1);
    //        vertices.Add(p2);
    //        vertices.Add(p3);
    //    }


        
    //    for (int i = 0; i < triesToDraw; i++)
    //    {



    //        string stri = string.Empty;


    //        Vector3 p1 = mesh.vertices[mesh.triangles[i + 0]];
    //        Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
    //        Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];




    //    }

    

    //        mesh.vertices = vertices.ToArray();


    //        mesh.triangles = triangles.ToArray();
    //        mesh.RecalculateNormals();

    //        this.transform.GetComponent<MeshRenderer>().material = material;

    //        string conString = string.Empty;



    //}





    public void UpdateMesh(Mesh toCopy)
    {
        mesh = new Mesh();

        GetComponent<MeshFilter>().mesh = mesh;

        mesh.Clear();

        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();



        int triesToDraw = 5;


        if (triesToDraw > toCopy.triangles.Length / 3)
        {
            triesToDraw = 12;
        }
        else if (triesToDraw <= 0)
        {
            triesToDraw = 1;
        }



        string vertices = string.Empty;

        foreach (var vertex in toCopy.vertices)
        {
            verticesList.Add(vertex);
            vertices += " " + vertex;
        }


        string triangle = string.Empty;

        int num = 0;
        foreach (var tris in toCopy.triangles)
        {
            num++;
            triangle += " " + tris;
        }



        for (int i = 0; i < triesToDraw * 3; i++)
        {
            trianglesList.Add(toCopy.triangles[i]);
        }

        var nd = toCopy.triangles[0];
        Debug.Log(nd);

        mesh.vertices = verticesList.ToArray();


        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();

        this.transform.GetComponent<MeshRenderer>().material = material;


        Debug.Log(mesh.subMeshCount);


    }
    public void MoveUp()
    {
        vertices[3] = new Vector3(vertices[3].x, vertices[3].y, vertices[3].z + 0.5f);




        // UpdateMesh();

    }










}
