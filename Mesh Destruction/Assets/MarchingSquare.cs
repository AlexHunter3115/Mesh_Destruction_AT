using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MarchingSquare : MonoBehaviour
{




    public int resolutionX = 40;
    public int resolutionY = 10;

    [SerializeField]
    public Vector3[,] verticesStatic = new Vector3[0, 0];
    MarchingSquarePoint[,] marchingPoints = new MarchingSquarePoint[0,0];

    MarchingSquarePoint[,] marchingPointsFlood = new MarchingSquarePoint[0,0];

    


    public bool reload;
    public bool genMarch;

    public GameObject topLeft;
    public GameObject topRight;
    public GameObject botRight;
    public GameObject botLeft;

    public Material mat;

    public bool disableGizmos;

    public float dist = 1.3f;   // inverse square law


    private void Start()
    {
        reload = true;
        genMarch = false;
        disableGizmos = true;
    }


    private void Update()
    {
        if (reload)
        {
            reload = false;

            Vector3 topLeftPos = topLeft.transform.position;
            Vector3 topRightPos = topRight.transform.position;
            Vector3 botLeftPos = botLeft.transform.position;
            Vector3 botRightPos = botRight.transform.position;

            verticesStatic = new Vector3[resolutionY + 1, resolutionX + 1];

            var yListLeft = new List<Vector3>();
            var yListRight = new List<Vector3>();

            for (int i = 0; i < resolutionY + 1; i++)
            {
                float lerpVal = (float)i / (float)resolutionY;
                yListLeft.Add(Vector3.Lerp(topLeftPos, botLeftPos, lerpVal));

                yListRight.Add(Vector3.Lerp(topRightPos, botRightPos, lerpVal));
            }

            for (int y = 0; y < yListLeft.Count; y++)
            {
                for (int x = 0; x < resolutionX + 1; x++)
                {
                    float lerpVal = (float)x / (float)resolutionX;
                    verticesStatic[y, x] = Vector3.Lerp(yListLeft[y], yListRight[y], lerpVal);
                }
            }

            marchingPoints = new MarchingSquarePoint[resolutionY + 1, resolutionX + 1];

            for (int y = 0; y < verticesStatic.GetLength(0); y++)
            {
                for (int x = 0; x < verticesStatic.GetLength(1); x++)
                {

                        marchingPoints[y, x] = new MarchingSquarePoint(verticesStatic[y, x], true, 1);

                    
                }
            }

            CallMarch();

        }



        if (genMarch)
        {
            genMarch = false;
            CallMarch();
        }
    }



    void OnMouseDown()
    {

        Ray ray = new Ray();
        RaycastHit hit;

        Vector3 mousePos = Input.mousePosition;

    
        ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 intersectionPoint = hit.point;

            foreach (var point in marchingPoints)
            {
                if (Vector3.Distance(intersectionPoint,point.position) <= dist) 
                {
                    Debug.Log("----------");
                    Debug.Log(point.weigth);


                    point.weigth -= Mathf.Pow(2, -(Vector3.Distance(intersectionPoint, point.position))-0.5f);
                    Debug.Log(Vector3.Distance(intersectionPoint, point.position));
                    Debug.Log(point.weigth);
                }
            }
            CallMarch();
        }
       
    }








    private void FloodFillSetup() 
    {
        
    
    }
















    private void CallMarch() 
    {

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();


        for (int y = 0; y < resolutionY; y++)
        {
            for (int x = 0; x < resolutionX; x++)
            {

                int binary = 0;

                var marchSquareTL = marchingPoints[y, x];    //8
                var marchSquareTR = marchingPoints[y, x + 1];   //4

                var marchSquareBL = marchingPoints[y + 1, x];   // 1
                var marchSquareBR = marchingPoints[y + 1, x + 1];    //2

                if (marchSquareTL.weigth >= 0.3f) { binary += 8; }
                if (marchSquareTR.weigth >= 0.3f) { binary += 4; }
                if (marchSquareBR.weigth >= 0.3f) { binary += 2; }
                if (marchSquareBL.weigth >= 0.3f) { binary += 1; }

                var addingList = DrawOrder(binary, marchSquareTL, marchSquareTR, marchSquareBL, marchSquareBR);


                for (int i = 0; i < addingList.Count; i++)
                {
                    trianglesList.Add(verticesList.Count);
                    verticesList.Add(addingList[i]);
                }
            }
        }

        Debug.Log(verticesList.Count);
        Debug.Log(trianglesList.Count);

        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();


        this.GetComponent<MeshRenderer>().material = mat;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        GetComponent<MeshCollider>().convex = false;
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }


    //altought here might not be like alot of tri, the arry only takes i think 60k but i am giving it 80k so need to figrue out a way



    private List<Vector3> DrawOrder(int activated, MarchingSquarePoint topLeft, MarchingSquarePoint topRight, MarchingSquarePoint botLeft, MarchingSquarePoint botRight)
    {
        List<Vector3> vertices = new List<Vector3>();


        Vector3 midLeft = Vector3.Lerp(topLeft.position, botLeft.position, topLeft.weigth/(topLeft.weigth + botLeft.weigth));
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
                Gizmos.DrawSphere(vertex, 0.1f);
            }
        }

    }
}




public class MarchingSquarePoint
{

    public Vector3 position;
    public bool state;
    public float weigth;


    public MarchingSquarePoint(Vector3 _position, bool _state,float _weight)
    {
        this.position = _position;
        this.state = _state;
        this.weigth = _weight;
    }
}