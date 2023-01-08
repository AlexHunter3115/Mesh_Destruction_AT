using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class MarchingSquare : MonoBehaviour
{
    [SerializeField]
    public Vector3[,] verticesStatic = new Vector3[0, 0];
    public MarchingSquarePoint[,] marchingPoints = new MarchingSquarePoint[0, 0];

    public List<MarchingSquarePoint> floodListMarching = new List<MarchingSquarePoint>();

    //public bool reload;
    //public bool genMarch;

    public bool disableGizmos;

    public GameObject mirrorWall;

    public bool inner = false;

    public float minimumWeight = 0.3f;

    public CreateMSQPlanes parentScript;

    private MarchingSquare otherWallMarchinSquare;
    private MeshFilter ownMeshFilter;



    private void Start()
    {
        CallMarch();

        otherWallMarchinSquare = mirrorWall.GetComponent<MarchingSquare>();
        ownMeshFilter = this.GetComponent<MeshFilter>();

        disableGizmos = true;
    }


    //might deete the wall check and just leave it, we could power down the projectile based on the travled distance
  /// <summary>
  /// Call this to receive a shoot to this plane and run the marhching cubes
  /// </summary>
  /// <param name="impactPoint"></param>
  /// <param name="distanceEffect"></param>
  /// <param name="direction"></param>
  /// <param name="wall">false if its needs to propegate</param>
    public void ImpactReceiver(Vector3 impactPoint, float distanceEffect, Vector3 direction, bool wall = false)
    {

        marchingPoints = otherWallMarchinSquare.marchingPoints;

        Debug.Log($"-----------------------------\n--------------------------\n\n");

        Stopwatch st = new Stopwatch();
        st.Start();
        foreach (var point in marchingPoints)
        {
            if (Vector3.Distance(impactPoint, point.position) <= distanceEffect)
            {
                point.weigth -= parentScript.weightDistribution.Evaluate(Vector3.Distance(impactPoint, point.position));
            }
        }


        if (!wall)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.TransformPoint(impactPoint), direction, out hit, Mathf.Infinity))
            {
                //Debug.DrawRay(transform.TransformPoint(impactPoint), direction * hit.distance, Color.yellow, 20);
                if (hit.transform.GetComponent<MarchingSquare>() != null)
                {
                    GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);

                    newRef.transform.position = hit.point;
                    newRef.transform.parent = hit.transform;

                    var marchComp = hit.transform.GetComponent<MarchingSquare>();

                    marchComp.ImpactReceiver(newRef.transform.localPosition, distanceEffect, direction, true);
                }
            }
        }


        CallMarch();
        FloodFillSetup();

        otherWallMarchinSquare.CopyMesh(ownMeshFilter.mesh);

        //StartCoroutine(MarchCo());
        st.Stop();

        Debug.Log($"<color=red>Performance: OVERALL operation took {st.ElapsedMilliseconds} ticks</color>");
    }


    private IEnumerator MarchCo()
    {


        CallMarch();

        FloodFillSetup();


        otherWallMarchinSquare.CopyMesh(ownMeshFilter.mesh);


        yield return null;
    }


    private void FloodFillSetup()
    {

        Stopwatch st = new Stopwatch();
        st.Start();


        foreach (var point in marchingPoints)
        {
            point.state = false;
        }

        //int iter = 0;

        List<Vector2Int> coords = new List<Vector2Int>();

        while (true)
        {
            //false means its not been added
            coords.Clear();
            floodListMarching.Clear();
            //if (iter > 10)
            //{
            //    break;
            //}

            bool done = true;

            foreach (var point in marchingPoints)
            {
                if (!point.state && point.weigth >= 0.3f)
                {
                    done = false;
                    coords.Add(point.idx);
                }
            }


            if (done)
                break;

           
            var wantedCoord = coords[Random.Range(0, coords.Count)];  //pick a random coord from the choosen ones

            FloodCall(wantedCoord.x, wantedCoord.y);

            if (floodListMarching.Count > 1)
            {
                bool toDel = true;

                foreach (var point in floodListMarching)
                {
                    if (point.idx.x == 0 || point.idx.y == 0 || point.idx.x == marchingPoints.GetLength(1) - 1 || point.idx.y == marchingPoints.GetLength(0) - 1)
                    {
                        toDel = false;
                        break;
                    }
                }

                if (toDel)
                {
                    var vectorOfVectors = new List<Vector3>();

                    int counter = 0;

                    foreach (var point in floodListMarching)
                    {
                        if (parentScript.voronoiNum < 2)
                        {

                            if (point.weigth <= 0.95f)
                            {
                                vectorOfVectors.Add(point.position);

                                if (this.transform.localPosition.x > 0)
                                    vectorOfVectors.Add(new Vector3(point.position.x - 0.05f, point.position.y, point.position.z));
                                else
                                    vectorOfVectors.Add(new Vector3(point.position.x + 0.05f, point.position.y, point.position.z));
                            }
                            else
                            {
                                counter++;
                            }
                        }
                        else 
                        {
                            vectorOfVectors.Add(point.position);
                            if (this.transform.localPosition.x > 0)
                                vectorOfVectors.Add(new Vector3(point.position.x - 0.05f, point.position.y, point.position.z));
                            else
                                vectorOfVectors.Add(new Vector3(point.position.x + 0.05f, point.position.y, point.position.z));
                        }

                        point.weigth = 0;
                    }


                    if(parentScript.voronoiNum > 1) 
                    {
                        var voroniOutcome = GeneralUtil.VoronoiDivision(vectorOfVectors, parentScript.voronoiNum);
                   
                        foreach (var voronoi in voroniOutcome)
                        {
                            var increOutcom = GeneralUtil.IncrementalConvex(voronoi);

                            FormObject(increOutcom.Item1, increOutcom.Item2, this.transform.localPosition.x > 0 ? true : false);
                        }
                    }
                    else 
                    {
                        var increOutcom = GeneralUtil.IncrementalConvex(vectorOfVectors);

                        FormObject(increOutcom.Item1, increOutcom.Item2, this.transform.localPosition.x > 0 ? true : false);
                    }



                    CallMarch();   
                }
            }

            //iter++;
        }

        st.Stop();

        Debug.Log($"<color=yellow>Performance: flood fill operation took {st.ElapsedMilliseconds} milliseconds</color>");

    }
    private void FloodCall(int x, int y)
    {
        if (y >= 0 && x >= 0 && y < marchingPoints.GetLength(0) && x < marchingPoints.GetLength(1))
        {
            if (!marchingPoints[y, x].state && marchingPoints[y, x].weigth >= 0.3f)
            {
                marchingPoints[y, x].state = true;

                floodListMarching.Add(marchingPoints[y, x]);

                FloodCall(x + 1, y);
                FloodCall(x - 1, y);
                FloodCall(x, y + 1);
                FloodCall(x, y - 1);
            }
        }
    }


    private void CallMarch()
    {
        Stopwatch st = new Stopwatch();
        st.Start();

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();


        for (int y = 0; y < parentScript.resolutionY; y++)
        {
            for (int x = 0; x < parentScript.resolutionX; x++)
            {

                int binary = 0;
                
                var marchSquareTL = marchingPoints[y, x];    //8
                var marchSquareTR = marchingPoints[y, x + 1];   //4

                var marchSquareBL = marchingPoints[y + 1, x];   // 1
                var marchSquareBR = marchingPoints[y + 1, x + 1];    //2

                if (marchSquareTL.weigth >= minimumWeight) { binary += 8; }
                if (marchSquareTR.weigth >= minimumWeight) { binary += 4; }
                if (marchSquareBR.weigth >= minimumWeight) { binary += 2; }
                if (marchSquareBL.weigth >= minimumWeight) { binary += 1; }

                var addingList = DrawOrder(binary, marchSquareTL, marchSquareTR, marchSquareBL, marchSquareBR);


                for (int i = 0; i < addingList.Count; i++)
                {
                    trianglesList.Add(verticesList.Count);
                    verticesList.Add(addingList[i]);
                }
            }
        }

        //Debug.Log(verticesList.Count);
        if (verticesList.Count > 65000)
        {
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        }

        if (inner)
        trianglesList.Reverse();

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();


        this.GetComponent<MeshRenderer>().material = parentScript.mat;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().convex = false;
        GetComponent<MeshCollider>().sharedMesh = mesh;


        st.Stop();

        Debug.Log($"<color=yellow>Performance: call march operation took {st.ElapsedMilliseconds} milliseconds</color>");




    }


    public void CopyMesh(Mesh meshToCopy)
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        //otherWallMarchinSquare.marchingPoints = marchingPoints;

        mesh.Clear();

        mesh.indexFormat = meshToCopy.indexFormat;

        var arr = meshToCopy.triangles.ToList();

        arr.Reverse();

        mesh.vertices = meshToCopy.vertices;
        mesh.triangles = arr.ToArray();

        this.GetComponent<MeshRenderer>().material = parentScript.mat;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().convex = false;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public void FormObject(List<Vector3> arrOfVec, List<int> tris, bool side)
    {

        if (arrOfVec.Count < 5)
            return;

        GameObject newPart = new GameObject();
        newPart.transform.position = this.transform.position;
        newPart.transform.rotation = this.transform.rotation;
        newPart.transform.localScale = this.transform.localScale;

        var mesh = new Mesh();
        mesh.name = "Fragment";

        mesh.vertices = arrOfVec.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();


        var renderer = newPart.AddComponent<MeshRenderer>();
        renderer.materials = this.GetComponent<MeshRenderer>().materials;

        var filter = newPart.AddComponent<MeshFilter>();
        filter.mesh = mesh;

        var collider = newPart.AddComponent<MeshCollider>();
        collider.convex = true;

        var rigidbody = newPart.AddComponent<Rigidbody>();


        if (side)
            rigidbody.AddForce(this.transform.right * 2, ForceMode.Impulse);
        else
            rigidbody.AddForce(this.transform.right * -2, ForceMode.Impulse);

    }



    //altought here might not be like alot of tri, the arry only takes i think 60k but i am giving it 80k so need to figrue out a way

    private List<Vector3> DrawOrder(int activated, MarchingSquarePoint topLeft, MarchingSquarePoint topRight, MarchingSquarePoint botLeft, MarchingSquarePoint botRight)
    {
        List<Vector3> vertices = new List<Vector3>();


        Vector3 midLeft = Vector3.Lerp(topLeft.position, botLeft.position, topLeft.weigth / (topLeft.weigth + botLeft.weigth));
        Vector3 midRight = Vector3.Lerp(topRight.position, botRight.position, topRight.weigth / (topRight.weigth + botRight.weigth));
        Vector3 midTop = Vector3.Lerp(topLeft.position, topRight.position, topLeft.weigth / (topLeft.weigth + topRight.weigth));
        Vector3 midBot = Vector3.Lerp(botLeft.position, botRight.position, botLeft.weigth / (botLeft.weigth + botRight.weigth));


        switch (activated)
        {

            case 0:
                //nothing 
                break;

            case 1:
                vertices.Add(botLeft.position);
                vertices.Add(midLeft);
                vertices.Add(midBot);
                break;

            case 2:

                vertices.Add(botRight.position);
                vertices.Add(midBot);
                vertices.Add(midRight);
                break;

            case 3:

                vertices.Add(botLeft.position);
                vertices.Add(midLeft);
                vertices.Add(midRight);

                vertices.Add(botRight.position);
                vertices.Add(botLeft.position);
                vertices.Add(midRight);

                break;

            case 4:

                vertices.Add(midRight);
                vertices.Add(midTop);
                vertices.Add(topRight.position);
                break;

            case 5:


                vertices.Add(midRight);
                vertices.Add(midTop);
                vertices.Add(topRight.position);

                vertices.Add(midBot);
                vertices.Add(midTop);
                vertices.Add(midRight);

                vertices.Add(midBot);
                vertices.Add(botLeft.position);
                vertices.Add(midTop);

                vertices.Add(botLeft.position);
                vertices.Add(midLeft);
                vertices.Add(midTop);

                break;

            case 6:

                vertices.Add(botRight.position);
                vertices.Add(midBot);
                vertices.Add(midTop);

                vertices.Add(midTop);
                vertices.Add(topRight.position);
                vertices.Add(botRight.position);

                break;

            case 7:
                vertices.Add(botLeft.position);
                vertices.Add(midLeft);
                vertices.Add(midTop);

                vertices.Add(botLeft.position);
                vertices.Add(midTop);
                vertices.Add(botRight.position);

                vertices.Add(botRight.position);
                vertices.Add(midTop);
                vertices.Add(topRight.position);

                break;

            case 8:

                vertices.Add(midLeft);
                vertices.Add(topLeft.position);
                vertices.Add(midTop);

                break;

            case 9:

                vertices.Add(midBot);
                vertices.Add(botLeft.position);
                vertices.Add(topLeft.position);

                vertices.Add(midBot);
                vertices.Add(topLeft.position);
                vertices.Add(midTop);

                break;

            case 10:

                vertices.Add(midBot);
                vertices.Add(midLeft);
                vertices.Add(topLeft.position);

                vertices.Add(midBot);
                vertices.Add(topLeft.position);
                vertices.Add(botRight.position);

                vertices.Add(botRight.position);
                vertices.Add(topLeft.position);
                vertices.Add(midRight);

                vertices.Add(topLeft.position);
                vertices.Add(midTop);
                vertices.Add(midRight);

                break;

            case 11:

                vertices.Add(botRight.position);
                vertices.Add(botLeft.position);
                vertices.Add(topLeft.position);

                vertices.Add(botRight.position);
                vertices.Add(topLeft.position);
                vertices.Add(midRight);

                vertices.Add(midRight);
                vertices.Add(topLeft.position);
                vertices.Add(midTop);

                break;

            case 12:

                vertices.Add(midRight);
                vertices.Add(midLeft);
                vertices.Add(topLeft.position);

                vertices.Add(midRight);
                vertices.Add(topLeft.position);
                vertices.Add(topRight.position);

                break;

            case 13:

                vertices.Add(midBot);
                vertices.Add(topLeft.position);
                vertices.Add(midRight);

                vertices.Add(midBot);
                vertices.Add(botLeft.position);
                vertices.Add(topLeft.position);

                vertices.Add(midRight);
                vertices.Add(topLeft.position);
                vertices.Add(topRight.position);

                break;

            case 14:

                vertices.Add(midBot);
                vertices.Add(midLeft);
                vertices.Add(topLeft.position);

                vertices.Add(midBot);
                vertices.Add(topLeft.position);
                vertices.Add(botRight.position);

                vertices.Add(botRight.position);
                vertices.Add(topLeft.position);
                vertices.Add(topRight.position);

                break;
            case 15:

                vertices.Add(botRight.position);
                vertices.Add(botLeft.position);
                vertices.Add(topLeft.position);

                vertices.Add(botRight.position);
                vertices.Add(topLeft.position);
                vertices.Add(topRight.position);

                break;
            default:

                break;
        }


        return vertices;


    }

    private void OnDrawGizmos()
    {

        if (!disableGizmos)
        {
            foreach (var vertex in verticesStatic)
            {
                Gizmos.DrawSphere(vertex, 0.01f);
            }
        }
    }
}




public class MarchingSquarePoint
{

    public Vector3 position;
    public bool state;
    public float weigth;
    public Vector2Int idx;


    public MarchingSquarePoint(Vector3 _position, bool _state, float _weight, Vector2Int _idx)
    {
        this.position = _position;
        this.state = false;
        this.weigth = _weight;
        this.idx = _idx;
    }
}



