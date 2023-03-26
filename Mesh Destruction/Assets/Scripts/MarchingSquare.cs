using System;
using System.Collections.Generic;
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

    public GameObject mirrorWall;

    public bool inner = false;

    public float minimumWeight = 0.3f;

    public CreateMSQPlanes parentScript;

    private MarchingSquare otherWallMarchinSquare;
    private MeshFilter ownMeshFilter;

    public int chunkSize = 6;
    //private int CLength = 0;


    private void Start()
    {
        CallMarch();

        otherWallMarchinSquare = mirrorWall.GetComponent<MarchingSquare>();
        ownMeshFilter = this.GetComponent<MeshFilter>();

        ChunkCreate(chunkSize);
    }


    [HideInInspector]
    public List<Chunk> chunks = new List<Chunk>();


    public void ChunkCreate(int chunkSize)
    {
        for (int row = 0; row < marchingPoints.GetLength(0); row += chunkSize)
        {
            for (int col = 0; col < marchingPoints.GetLength(1); col += chunkSize)
            {
                chunks.Add(new Chunk());

                for (int i = 0; i < chunkSize; i++)
                {
                    for (int j = 0; j < chunkSize; j++)
                    {
                        if (row + i < marchingPoints.GetLength(0) && col + j < marchingPoints.GetLength(1))
                        {
                            chunks[chunks.Count - 1].listOfObjInChunk.Add(marchingPoints[row + i, col + j]);

                            if (i == 0 && j == 0) //first pass
                            {
                                chunks[chunks.Count - 1].bottomLeft = new Vector2Int(row + i, col + j);
                            }
                            else
                            {
                                chunks[chunks.Count - 1].topRight = new Vector2Int(row + i, col + j);
                            }
                        }
                    }
                }
            }
        }
    }


    //might deete the wall check and just leave it, i could power down the projectile based on the travled distance
    /// <summary>
    /// Call this to receive a shoot to this plane and run the marhching cubes
    /// </summary>
    /// <param name="impactPoint"></param>
    /// <param name="distanceEffect"></param>
    /// <param name="direction"></param>
    /// <param name="wall">false if its needs to propegate</param>
    public void ImpactReceiver(Vector3 impactPoint, float distanceEffect, Vector3 direction, float multi, AnimationCurve graph, bool wall = false)
    {
        marchingPoints = otherWallMarchinSquare.marchingPoints;

        int wantedIDX = -1;

        for (int i = 0; i < chunks.Count; i++)
        {
            if (AABBCol(transform.TransformPoint(impactPoint), chunks[i]))
            {
                wantedIDX = i;
                break;
            }
        }

        if (wantedIDX == -1)  // if the chunk system faild do everything
        {
            foreach (var point in marchingPoints)
            {
                if (Vector3.Distance(impactPoint, point.position) <= distanceEffect)
                {
                    point.weigth -= ((graph.Evaluate(Vector3.Distance(impactPoint, point.position))) * multi) * parentScript.strengthOfObject;
                }
            }
        }
        else
        {
            var indexesToDraw = new List<int>();

            indexesToDraw.Add(wantedIDX);

            var size = Mathf.CeilToInt((float)marchingPoints.GetLength(0) / (float)chunkSize);

            indexesToDraw.Add(wantedIDX - 1);
            indexesToDraw.Add(wantedIDX + 1);

            indexesToDraw.Add(wantedIDX + size);
            indexesToDraw.Add(wantedIDX - size);

            indexesToDraw.Add(indexesToDraw[4] + 1);
            indexesToDraw.Add(indexesToDraw[4] - 1);
            indexesToDraw.Add(indexesToDraw[3] - 1);
            indexesToDraw.Add(indexesToDraw[3] + 1);

            foreach (var chunkIndex in indexesToDraw)
            {
                if (chunkIndex < 0 || chunkIndex >= chunks.Count)
                {
                    continue;
                }
                foreach (var point in chunks[chunkIndex].listOfObjInChunk)
                {
                    if (Vector3.Distance(impactPoint, marchingPoints[point.idx.y, point.idx.x].position) <= distanceEffect)
                    {
                        marchingPoints[point.idx.y, point.idx.x].weigth -= (graph.Evaluate(Vector3.Distance(impactPoint, marchingPoints[point.idx.y, point.idx.x].position))) * multi;
                    }
                }
            }
        }

        if (!wall)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.TransformPoint(impactPoint), direction, out hit, Mathf.Infinity))
            {
                if (hit.transform.GetComponent<MarchingSquare>() != null)
                {
                    GameObject newRef = Instantiate(PlayerScript.instance.bulletPrefab);

                    newRef.transform.position = hit.point;
                    newRef.transform.parent = hit.transform;

                    var marchComp = hit.transform.GetComponent<MarchingSquare>();

                    marchComp.ImpactReceiver(newRef.transform.localPosition, distanceEffect, direction, multi * parentScript.energyLossMultiplier, graph, true);
                }
            }
        }

        CallMarch();
        FloodFillSetup();

        otherWallMarchinSquare.CopyMesh(ownMeshFilter.mesh);
    }

    private bool AABBCol(Vector3 player, Chunk chunk)
    {
        int correctBLx = chunk.bottomLeft.x >= marchingPoints.GetLength(1) ? marchingPoints.GetLength(1) - 1 : chunk.bottomLeft.x;
        int correctTRx = chunk.topRight.x >= marchingPoints.GetLength(1) ? marchingPoints.GetLength(1) - 1 : chunk.topRight.x;
        int correctBLy = chunk.bottomLeft.y >= marchingPoints.GetLength(0) ? marchingPoints.GetLength(0) - 1 : chunk.bottomLeft.y;
        int correctTRy = chunk.topRight.y >= marchingPoints.GetLength(0) ? marchingPoints.GetLength(0) - 1 : chunk.topRight.y;

        var topRight = transform.parent.TransformPoint(marchingPoints[correctTRx, correctTRy].position);
        var botLeft = transform.parent.TransformPoint(marchingPoints[correctBLx, correctBLy].position);

        if (player.y >= botLeft.y && player.y < topRight.y)
        {
            if (player.z >= botLeft.z && player.z < topRight.z)
            {
                return true;
            }
        }
        return false;
    }

    private void FloodFillSetup()
    {
        foreach (var point in marchingPoints)
        {
            point.state = false;
        }

        int iter = 0;

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
            //FloodFill(new Vector2Int( wantedCoord.x, wantedCoord.y));

            if (floodListMarching.Count > 1)
            {
                bool toDel = true;

                for (int i = 0; i < floodListMarching.Count; i++)
                {
                    if (floodListMarching[i].idx.x == 0 || floodListMarching[i].idx.y == 0 || floodListMarching[i].idx.x == marchingPoints.GetLength(1) - 1 || floodListMarching[i].idx.y == marchingPoints.GetLength(0) - 1)
                    {
                        toDel = false;
                        break;
                    }
                }

                //Debug.Log(floodListMarching.Count);

               // Debug.Log($"{toDel}");

                if (toDel)
                {
                    var vectorOfVectors = new List<Vector3>();

                    int counter = 0;

                    for (int i = 0; i < floodListMarching.Count; i++)
                    {
                        if (parentScript.voronoiNum < 2)
                        {

                            if (floodListMarching[i].weigth <= 0.95f)
                            {
                                vectorOfVectors.Add(floodListMarching[i].position);

                                if (this.transform.localPosition.x > 0)
                                    vectorOfVectors.Add(new Vector3(floodListMarching[i].position.x - 0.05f, floodListMarching[i].position.y, floodListMarching[i].position.z));
                                else
                                    vectorOfVectors.Add(new Vector3(floodListMarching[i].position.x + 0.05f, floodListMarching[i].position.y, floodListMarching[i].position.z));
                            }
                            else
                            {
                                counter++;
                            }
                        }
                        else
                        {
                            vectorOfVectors.Add(floodListMarching[i].position);
                            if (this.transform.localPosition.x > 0)
                                vectorOfVectors.Add(new Vector3(floodListMarching[i].position.x - 0.05f, floodListMarching[i].position.y, floodListMarching[i].position.z));
                            else
                                vectorOfVectors.Add(new Vector3(floodListMarching[i].position.x + 0.05f, floodListMarching[i].position.y, floodListMarching[i].position.z));
                        }
                        floodListMarching[i].weigth = 0;
                    }


                    if (parentScript.voronoiNum > 1)
                    {
                        var voroniOutcome = GeneralUtil.VoronoiDivision(vectorOfVectors, parentScript.voronoiNum);

                        for (int i = 0; i < voroniOutcome.Length; i++)
                        {
                            if (voroniOutcome[i].Count > 5)
                            {
                                var increOutcom = GeneralUtil.IncrementalConvex(voroniOutcome[i]);

                                if (increOutcom != null)
                                    FormObject(increOutcom.Item1, increOutcom.Item2, this.transform.localPosition.x > 0 ? true : false);
                            }
                        }
                    }
                    else
                    {
                        var increOutcom = GeneralUtil.IncrementalConvex(vectorOfVectors);

                        if (increOutcom != null)
                            FormObject(increOutcom.Item1, increOutcom.Item2, this.transform.localPosition.x > 0 ? true : false);
                    }

                    CallMarch();
                }
            }

            iter++;
        }

        //Debug.Log($"<color=yellow>Performance: flood fill operation took {st.ElapsedMilliseconds} milliseconds</color>");
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

        Vector2[] uvs = new Vector2[mesh.vertices.Length]; // create a new array of UV coordinates

        // set the UV coordinates for each vertex
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z); // set the UV coordinates based on the vertex position
        }

        mesh.uv = uvs;

        if (parentScript.typeOfFragElimination)
        {
            var delPart = newPart.AddComponent<ShrinkOnCol>();
            delPart.dissapearTimer = parentScript.fragElimTimer;
        }
        else
        {
            var delPart = newPart.AddComponent<DelColOnCol>();
            delPart.dissapearTimer = parentScript.fragElimTimer;
        }


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




}


[Serializable]
public class Chunk
{
    public Vector2Int topRight = Vector2Int.zero;
    public Vector2Int bottomLeft = Vector2Int.zero;

    public int width;
    public int height;

    public int index = 0;
    public List<MarchingSquarePoint> listOfObjInChunk = new List<MarchingSquarePoint>();
}

public class MarchingSquarePoint
{
    public Vector3 position;
    public bool state;
    public float weigth;
    public Vector2Int idx;
    public int chunkIdx;


    public MarchingSquarePoint(Vector3 _position, bool _state, float _weight, Vector2Int _idx)
    {
        this.position = _position;
        this.state = false;
        this.weigth = _weight;
        this.idx = _idx;
    }
}



