using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMSQPlanes : MonoBehaviour
{


    public GameObject topLeft;
    public GameObject topRight;
    public GameObject botRight;
    public GameObject botLeft;


    public GameObject innerWall2;
    public GameObject outerWall2;

    public GameObject outerWall1;
    public GameObject innerWall1;

    public float distance = 0.1f;

    [Range(0.1f,0.95f)]
    public float energyLossMultiplier;

    public int resolutionX = 60;
    public int resolutionY = 60;

    public int voronoiNum = 0;

    public Material mat;


    [Tooltip("True for shrinking animation, False for fall through the ground")]
    public bool typeOfFragElimination;
    public float fragElimTimer = 2;

    /// <summary>
    /// x is distance 
    /// and the y is the damage
    /// </summary>
    //[SerializeField]
    //public AnimationCurve weightDistribution = new AnimationCurve();


    // Start is called before the first frame update
    void Start()
    {

        #region innerWall2

        innerWall2 = new GameObject("innerWall2");
        innerWall2.transform.parent = this.transform;
        
        innerWall2.AddComponent<MeshRenderer>();
        innerWall2.AddComponent<MeshFilter>();
        innerWall2.AddComponent<MeshCollider>();
        var march = innerWall2.AddComponent<MarchingSquare>();
        march.inner = true;
        march.parentScript = this;

        march.mirrorWall = outerWall2;

        innerWall2.transform.localPosition =  new Vector3(-distance,0,0);
        innerWall2.layer = LayerMask.NameToLayer("innerWall");

        Vector3 topLeftPos = topLeft.transform.localPosition;   //loc
        Vector3 topRightPos = topRight.transform.localPosition ;
        Vector3 botLeftPos = botLeft.transform.localPosition ;
        Vector3 botRightPos = botRight.transform.localPosition ;

        march.verticesStatic = new Vector3[resolutionY + 1, resolutionX + 1];

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
                march.verticesStatic[y, x] = Vector3.Lerp(yListLeft[y], yListRight[y], lerpVal);
            }
        }

        march.marchingPoints = new MarchingSquarePoint[resolutionY + 1, resolutionX + 1];

        for (int y = 0; y < march.verticesStatic.GetLength(0); y++)
        {
            for (int x = 0; x < march.verticesStatic.GetLength(1); x++)
            {
                march.marchingPoints[y, x] = new MarchingSquarePoint(march.verticesStatic[y, x], true, 1, new Vector2Int(x, y));
            }
        }

        innerWall2.transform.rotation = new Quaternion(0,0,0,0);

        #endregion



        #region innerWall1

        innerWall1 = new GameObject("innerWall1");
        innerWall1.transform.parent = this.transform;
        innerWall1.AddComponent<MeshRenderer>();
        innerWall1.AddComponent<MeshFilter>();
        innerWall1.AddComponent<MeshCollider>();
        march = innerWall1.AddComponent<MarchingSquare>();
        march.parentScript = this;
        march.inner = false;

     
        march.mirrorWall = outerWall1;

        innerWall1.transform.localPosition = new Vector3(distance, 0, 0);
        innerWall1.layer = LayerMask.NameToLayer("innerWall");


         topLeftPos = topLeft.transform.localPosition;   //loc
         topRightPos = topRight.transform.localPosition;
         botLeftPos = botLeft.transform.localPosition;
         botRightPos = botRight.transform.localPosition;

        march.verticesStatic = new Vector3[resolutionY + 1, resolutionX + 1];

         yListLeft = new List<Vector3>();
         yListRight = new List<Vector3>();

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
                march.verticesStatic[y, x] = Vector3.Lerp(yListLeft[y], yListRight[y], lerpVal);
            }
        }

        march.marchingPoints = new MarchingSquarePoint[resolutionY + 1, resolutionX + 1];

        for (int y = 0; y < march.verticesStatic.GetLength(0); y++)
        {
            for (int x = 0; x < march.verticesStatic.GetLength(1); x++)
            {
                march.marchingPoints[y, x] = new MarchingSquarePoint(march.verticesStatic[y, x], true, 1, new Vector2Int(x, y));
            }
        }

        innerWall1.transform.rotation = new Quaternion(0, 0, 0, 0);
        #endregion



        #region outerWall1

        outerWall1 = new GameObject("outerWall1");
        outerWall1.transform.parent = this.transform;
        outerWall1.AddComponent<MeshRenderer>();
        outerWall1.AddComponent<MeshFilter>();
        outerWall1.AddComponent<MeshCollider>();
        march = outerWall1.AddComponent<MarchingSquare>();
        march.parentScript = this;
        march.inner = true;
        march.mirrorWall = innerWall1;

        outerWall1.transform.localPosition = new Vector3(distance, 0, 0);
        outerWall1.layer = LayerMask.NameToLayer("outerWall");

        topLeftPos = topLeft.transform.localPosition;   //loc
        topRightPos = topRight.transform.localPosition;
        botLeftPos = botLeft.transform.localPosition;
        botRightPos = botRight.transform.localPosition;

        march.verticesStatic = new Vector3[resolutionY + 1, resolutionX + 1];

        yListLeft = new List<Vector3>();
        yListRight = new List<Vector3>();

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
                march.verticesStatic[y, x] = Vector3.Lerp(yListLeft[y], yListRight[y], lerpVal);
            }
        }

        march.marchingPoints = new MarchingSquarePoint[resolutionY + 1, resolutionX + 1];

        for (int y = 0; y < march.verticesStatic.GetLength(0); y++)
        {
            for (int x = 0; x < march.verticesStatic.GetLength(1); x++)
            {
                march.marchingPoints[y, x] = new MarchingSquarePoint(march.verticesStatic[y, x], true, 1, new Vector2Int(x, y));
            }
        }


        outerWall1.transform.rotation = new Quaternion(0, 0, 0, 0);

        #endregion



        #region outerWall2

        outerWall2 = new GameObject("outerWall2");
        outerWall2.transform.parent = this.transform;
        outerWall2.AddComponent<MeshRenderer>();
        outerWall2.AddComponent<MeshFilter>();
        outerWall2.AddComponent<MeshCollider>();
        march = outerWall2.AddComponent<MarchingSquare>();
        march.mirrorWall = innerWall2;

        march.parentScript = this;
        march.inner = false;

        outerWall2.transform.localPosition = new Vector3(-distance, 0, 0);
        outerWall2.layer = LayerMask.NameToLayer("outerWall");

        topLeftPos = topLeft.transform.localPosition;   //loc
        topRightPos = topRight.transform.localPosition;
        botLeftPos = botLeft.transform.localPosition;
        botRightPos = botRight.transform.localPosition;


        march.verticesStatic = new Vector3[resolutionY + 1, resolutionX + 1];

        yListLeft = new List<Vector3>();
        yListRight = new List<Vector3>();

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
                march.verticesStatic[y, x] = Vector3.Lerp(yListLeft[y], yListRight[y], lerpVal);
            }
        }

        march.marchingPoints = new MarchingSquarePoint[resolutionY + 1, resolutionX + 1];

        for (int y = 0; y < march.verticesStatic.GetLength(0); y++)
        {
            for (int x = 0; x < march.verticesStatic.GetLength(1); x++)
            {
                march.marchingPoints[y, x] = new MarchingSquarePoint(march.verticesStatic[y, x], true, 1, new Vector2Int(x, y));
            }
        }



        outerWall2.transform.rotation = new Quaternion(0, 0, 0, 0);

        #endregion



        march = innerWall2.GetComponent<MarchingSquare>();
        march.mirrorWall = outerWall2;



        march = innerWall1.GetComponent<MarchingSquare>();
        march.mirrorWall = outerWall1;
    }


    
}
