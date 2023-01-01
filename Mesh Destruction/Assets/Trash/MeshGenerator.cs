using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static DeluTrig;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.Image;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    /*
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





    */

    /*

    the class holds a way to spawn the object from the parts and all the comp added








    there is class which holds the current parts part
    and then copy what is being cut into it making it a part
    for every cut or part,, this all depends i we want to do one cut at the time 

    create the pplane and send it to a function with the part to cut  -- done
    the function also takes the ddirection bool so we do up and down in respect to the plane   -- done
    for call of the function create anew part var where the part is going to be stored -- done

    have the two rays  -- done

    the thing is the script goes thgouht the material and shit like that but no need   -- ok

    then go through all the toiangles  in a loop +3

    use the get side fro the plane to see where the original.vertices[i] is in relation to the plane

    if all on one side then add it to the partmesh

    dependoing on the one that its on its own start the the rays from there to the other ones
    

    so plane raycast does this (This function sets enter to the distance along the ray, where it intersects the plane.)
    so that and out a point and then get a perc to then use a lerp fromt he dist 


    and then there si something about adding edges?
    there is this edges shit i dont get it i can just add the thing 

    and then add the triangles depening on how many were intersected 

    end of loop

    // to go through tris use 3++
     */



    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Plane edgePlane = new Plane();



    private void Start()
    {
        //bool a = false;
        //bool b = false;
        //bool c = true;


        //// if a is true then its = ot 0  else



        //// if this     than this    else that
        ////int something = a;
        //// print(something);
        //var somthing = 0;
        //var singleIndex = 0;
        //// is b == c  then returnt 0      if not then is a == c  yes return 1 else 2

        //a = false;
        //b = true;
        //c = true;




        //if (b == c)
        //{
        //    somthing = 0;
        //}
        //else
        //{
        //    if (a == c)
        //    {
        //        somthing = 1;
        //    }
        //    else
        //    {
        //        somthing = 2;
        //    }
        //}

        //singleIndex =     b == c ? 0 :         a == c ? 1 : 2;

        //print(singleIndex);
        //print(somthing);


        //a = false;
        //b = true;
        //c = false;

        //if (b == c)
        //{
        //    somthing = 0;
        //}
        //else
        //{
        //    if (a == c)
        //    {
        //        somthing = 1;
        //    }
        //    else
        //    {
        //        somthing = 2;
        //    }
        //}

        //singleIndex = b == c ? 0 : a == c ? 1 : 2;

        //print(singleIndex);
        //print(somthing);



        //a = false;
        //b = false;
        //c = true;

        //if (b == c)
        //{
        //    somthing = 0;
        //}
        //else
        //{
        //    if (a == c)
        //    {
        //        somthing = 1;
        //    }
        //    else
        //    {
        //        somthing = 2;
        //    }
        //}

        //singleIndex = b == c ? 0 : a == c ? 1 : 2;

        //print(singleIndex);
        //print(somthing);


        ////print(singleIndex);
        //// b == c expected   0      result
        //// b != c    a == c   expected 1     result
        //// b != c    a != c    expected 2    result


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

            CutCall(1);
        }
    }

    //prob needs to be here due to get comp and monobehvaiour stuff 
    public GameObject planeCutter;

    public void CutCall(int cuts)
    {

        //for now cuts is going to be 1


        MeshPart originalPart = new MeshPart();
        originalPart.InitPart(this.gameObject);    // initialise the part and var


        Mesh planeMesh = planeCutter.transform.GetComponent<MeshFilter>().mesh;

        Bounds bounds = this.transform.GetComponent<MeshFilter>().mesh.bounds;
        //Mesh planeMesh = planeCutter.transform.GetComponent<MeshFilter>().mesh;

        var plane = new Plane(Random.onUnitSphere, new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                                                                 Random.Range(bounds.min.y, bounds.max.y),
                                                                                 Random.Range(bounds.min.z, bounds.max.z)));



        var partA = CreatePart(originalPart, plane, false);
        var partB = CreatePart(originalPart, plane, true);




        partA.FormObject(this);
        partB.FormObject(this);

        Destroy(gameObject);
    }






    public MeshPart CreatePart(MeshPart originalPart, Plane planeCutter, bool direction)
    {
        MeshPart createdPart = new MeshPart();

        edgeSet = false;
        //bool firstCall = false;
        Vector3 fillSavedVertex = Vector3.zero;

        Ray ray1 = new Ray();
        Ray ray2 = new Ray();

        for (int i = 0; i < originalPart.triangleArr.Length; i = i + 3)
        {
            Vector3 vertexA = originalPart.verticesArr[originalPart.triangleArr[i]];
            Vector3 vertexB = originalPart.verticesArr[originalPart.triangleArr[i + 1]];
            Vector3 vertexC = originalPart.verticesArr[originalPart.triangleArr[i + 2]];
            int verticesEffected = 0;

            bool isVertexaAbove = planeCutter.GetSide(vertexA) == direction;  // get the confirmation if the point are above or below in respect to the plane, the direction changes based on the each of the parts
            bool isVertexbAbove = planeCutter.GetSide(vertexB) == direction;
            bool isVertexcAbove = planeCutter.GetSide(vertexC) == direction;

            if (isVertexaAbove) { verticesEffected++; }
            if (isVertexbAbove) { verticesEffected++; }
            if (isVertexcAbove) { verticesEffected++; }

            // all on one side therefore no need to change anything just add to the meshPart as they will be unchanged if applicable
            if (verticesEffected == 0) { continue; }  // will add this in the next direction call
            if (verticesEffected == 3) { createdPart.AddTriangle(vertexA, vertexB, vertexC, originalPart.Normals[originalPart.triangleArr[i]], originalPart.Normals[originalPart.triangleArr[i + 1]], originalPart.Normals[originalPart.triangleArr [i + 2]]); continue; } // all vertices are on one side and are added in this call of direction


            //get the vertice that is on its own to cast the rays towards the other two
            int aloneVertexIndex = 0;


            if (isVertexbAbove == isVertexcAbove)
            {
                aloneVertexIndex = 0;
            }
            else
            {
                if (isVertexaAbove == isVertexcAbove)
                {
                    aloneVertexIndex = 1;
                }
                else
                {
                    aloneVertexIndex = 2;
                }
            }


            float intersection2;
            float intersection1;
            // build the raycast
            ray1.origin = originalPart.verticesArr[originalPart.triangleArr[i + aloneVertexIndex]];  //origin point of the ray is the alone vertex
            ray2.origin = originalPart.verticesArr[originalPart.triangleArr[i + aloneVertexIndex]];

            // vector3 - vector3 = lookatDir
            var dir = originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]] - originalPart.verticesArr[originalPart.triangleArr[i + aloneVertexIndex]];
            ray1.direction = dir;
            planeCutter.Raycast(ray1, out intersection1);  // intersect = to the distance along the ray, where it intersects the plane
            float percentageAlongRay1 = intersection1 / dir.magnitude;


            // vector3 - vector3 = lookatDir
            dir = originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]] - originalPart.verticesArr[originalPart.triangleArr[i + aloneVertexIndex]];
            ray2.direction = dir;
            planeCutter.Raycast(ray2, out intersection2);  // intersect = to the distance along the ray, where it intersects the plane
            float percentageAlongRay2 = intersection2 / dir.magnitude;

            //fill in the created blank 

            AddEdge(createdPart, direction ? planeCutter.normal * -1f : planeCutter.normal,
                        ray1.origin + ray1.direction.normalized * intersection1,
                        ray2.origin + ray2.direction.normalized * intersection2);













            //if (firstCall == false)
            //{
            //    edgeVertex = ray1.origin + ray1.direction.normalized * intersection1;
            //    firstCall = true;
            //}
            //else
            //{
            //    Vector3 normal = direction ? planeCutter.normal * -1f : planeCutter.normal;

            //    //    this is to fill the hole thats created in the obj, the direction? planeCutter.normal * -1f : planeCutter.normal is to give the direction of the nomral in respect from the cuttin plane



            //    createdPart.AddTriangle(fillSavedVertex,
            //                            planeCutter.GetSide(edgeVertex + normal) ? ray1.origin + ray1.direction.normalized * intersection1 : ray2.origin + ray2.direction.normalized * intersection2,
            //                            planeCutter.GetSide(edgeVertex + normal) ? ray2.origin + ray2.direction.normalized * intersection2 : ray1.origin + ray1.direction.normalized * intersection1,
            //                            normal, normal, normal);
            //}











            //depedning on the vertex alone and the direction either a new triangle is formed or a trapezium is formed
            if (verticesEffected == 1)
            {
                createdPart.AddTriangle(
                                    originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex) % 3)]],
                                    ray1.GetPoint(intersection1),
                                    ray2.GetPoint(intersection2),
                                    originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]],
                                    Vector3.Lerp(originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]], originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]], intersection1),
                                    Vector3.Lerp(originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]], originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]], intersection2));
                continue;
            }
            if (verticesEffected == 2)
            {
                createdPart.AddTriangle(
                                    ray1.GetPoint(intersection1),
                                    originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]],
                                    originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]],
                                    Vector3.Lerp(originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]], originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]], intersection1),
                                    originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]],
                                    originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]]);


                createdPart.AddTriangle(
                                    ray1.GetPoint(intersection1),
                                    originalPart.verticesArr[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]],
                                    ray2.GetPoint(intersection2),
                                    Vector3.Lerp(originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]], originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 1) % 3)]], intersection1),
                                    originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]],
                                    Vector3.Lerp(originalPart.Normals[originalPart.triangleArr[i + aloneVertexIndex]], originalPart.Normals[originalPart.triangleArr[i + ((aloneVertexIndex + 2) % 3)]], intersection2));
                continue;
            }
        }
        return createdPart;
    }








    private void AddEdge(MeshPart partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vertex1;
        }
        else
        {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);

            partMesh.AddTriangle(
                                edgeVertex,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,  normal,
                                normal,
                                normal);
        }
    }




    public class MeshPart
    {
        //might need uvs and stuff like that but dont care
        public GameObject self;
        public Mesh mesh;

        public List<int> triangles = new List<int>();
        public int[] triangleArr = new int[0];

        public List<Vector3> vertices = new List<Vector3>();
        public Vector3[] verticesArr = new Vector3[0];


        private List<Vector3> _Normals = new List<Vector3>();
        public Vector3[] Normals;



        public MeshPart() { }   // constructor

        public void InitPart(GameObject partToInitialize)
        {
            self = partToInitialize;
            mesh = partToInitialize.transform.GetComponent<MeshFilter>().mesh;

            triangleArr = mesh.triangles;
            verticesArr = mesh.vertices;
            Normals = mesh.normals;
        }


        //add the normals bit as well
        public void AddTriangle(Vector3 vertexA, Vector3 vertexB, Vector3 vertexC, Vector3 normal1, Vector3 normal2, Vector3 normal3)
        {

            // adds a new tri for every call, this explains why the vertices are so hard to decipher 

            triangles.Add(vertices.Count);
            vertices.Add(vertexA);

            triangles.Add(vertices.Count);
            vertices.Add(vertexB);

            triangles.Add(vertices.Count);
            vertices.Add(vertexC);

            _Normals.Add(normal1);
            _Normals.Add(normal2);
            _Normals.Add(normal3);
        }



        public void FormObject(MeshGenerator original)
        {

            GameObject newPart = new GameObject();
            newPart.transform.position = original.transform.position;
            newPart.transform.rotation = original.transform.rotation;
            newPart.transform.localScale = original.transform.localScale;

            var mesh = new Mesh();
            mesh.name = original.GetComponent<MeshFilter>().mesh.name;

            mesh.vertices = vertices.ToArray();

            //mesh.RecalculateBounds();
            //mesh.RecalculateNormals();
            //mesh.RecalculateTangents();

            mesh.normals = Normals;

            mesh.triangles= triangles.ToArray();



            var renderer = newPart.AddComponent<MeshRenderer>();
            renderer.materials = original.GetComponent<MeshRenderer>().materials;

            var filter = newPart.AddComponent<MeshFilter>();
            filter.mesh = mesh;


            var collider = newPart.AddComponent<MeshCollider>();
            collider.convex = true;

            var rigidbody = newPart.AddComponent<Rigidbody>();
            var meshDestroy = newPart.AddComponent<MeshGenerator>();
            meshDestroy.planeCutter = original.planeCutter;


        }

    }



}









