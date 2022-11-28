using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class tetraDeluTrig : MonoBehaviour
{
    // this class was taken from my own Dissertation project



    [SerializeField]
    public List<Tetrahederal> Tetraingulation = new List<Tetrahederal>();

    public List<Vector3> pointsTest = new List<Vector3>();


    public List<Vector3> pointList = new List<Vector3>();



    public bool draw = false;
    public bool algo = false;



    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        if (draw)
        {


            if (!algo)
            {
                Gizmos.DrawSphere(pointsTest[0], 2f);
                Gizmos.DrawSphere(pointsTest[1], 2f);
                Gizmos.DrawSphere(pointsTest[2], 2f);
                Gizmos.DrawSphere(pointsTest[3], 2f);




                Gizmos.color = Color.yellow;

                Gizmos.DrawSphere(FindCircumcenter(pointsTest[0], pointsTest[1], pointsTest[2], pointsTest[3]), 4f);

            }
            else
            {

                Gizmos.DrawSphere(pointsTest[0], 2f);
                Gizmos.DrawSphere(pointsTest[1], 2f);
                Gizmos.DrawSphere(pointsTest[2], 2f);
                Gizmos.DrawSphere(pointsTest[3], 2f);

                Gizmos.color = Color.yellow;

                Gizmos.DrawSphere(Circumcentre(pointsTest[0], pointsTest[1], pointsTest[2], pointsTest[3]), 4f);

            }





        }
        else 
        {




            foreach (var point in pointList)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }



            foreach (var tetra in Tetraingulation)
            {
                foreach (var tri in tetra.triangles)
                {
                    Gizmos.DrawLine(tri.a, tri.b);
                    Gizmos.DrawLine(tri.b, tri.c);
                    Gizmos.DrawLine(tri.c, tri.a);
                }
            }
        
        }
    }



    public Vector3 Circumcentre(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Calculate plane for edge AB
        Vector3 planeABNormal = b - a;
        Vector3 planeABPoint = Vector3.Lerp(a, b, 0.5f);

        // Calculate plane for edge AC
        Vector3 planeACNormal = c - a;
        Vector3 planeACPoint = Vector3.Lerp(a, c, 0.5f);

        // Calculate plane for edge BD
        Vector3 planeBDNormal = d - b;
        Vector3 planeBDPoint = Vector3.Lerp(b, d, 0.5f);

        // Calculate plane for edge CD
        Vector3 planeCDNormal = d - c;
        Vector3 planeCDPoint = Vector3.Lerp(c, d, 0.5f);

        // Calculate line that is the plane-plane intersection between AB and AC
        Vector3 linePoint1;
        Vector3 lineDirection1;

        // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
        PlanePlaneIntersection(out linePoint1, out lineDirection1, planeABNormal, planeABPoint, planeACNormal, planeACPoint);

        Vector3 linePoint2;
        Vector3 lineDirection2;

        PlanePlaneIntersection(out linePoint2, out lineDirection2, planeBDNormal, planeBDPoint, planeCDNormal, planeCDPoint);

        // Calculate the point that is the plane-line intersection between the above line and CD
        Vector3 intersection;

        // Floating point inaccuracy often causes these two lines to not intersect, in that case get the two closest points on each line 
        // and average them
        // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
        if (!LineLineIntersection(out intersection, linePoint1, lineDirection1, linePoint2, lineDirection2))
        {
            Vector3 closestLine1;
            Vector3 closestLine2;

            // Taken from: http://wiki.unity3d.com/index.php/3d_Math_functions
            ClosestPointsOnTwoLines(out closestLine1, out closestLine2, linePoint1, lineDirection1, linePoint2, lineDirection2);

            // Intersection is halfway between the closest two points on lines
            intersection = Vector3.Lerp(closestLine2, closestLine2, 0.5f);
        }

        return intersection;
    }

    public  bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {

            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }

        else
        {
            return false;
        }
    }

    public  bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public  bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
    {

        linePoint = Vector3.zero;
        lineVec = Vector3.zero;

        //We can get the direction of the line of intersection of the two planes by calculating the 
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet. We need a point for that to go with the line vector.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.		
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float denominator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
        if (Mathf.Abs(denominator) > 0.006f)
        {

            Vector3 plane1ToPlane2 = plane1Position - plane2Position;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }




    private void Start()
    {


        List<Vector3> pointList = new List<Vector3>();


        for (int i = 0; i < 30; i++)
        {

            Vector3 point = new Vector3(Random.Range(1, 20), Random.Range(1, 20), Random.Range(1,20));


           pointList.Add(point); 

        }

        Debug.Log(pointList.Count);








        Tetraingulation = new List<Tetrahederal>();

        Vector3 superTriangleA = new Vector3(10000, 10000,0);
        Vector3 superTriangleB = new Vector3(10000, 0, 10000);
        Vector3 superTriangleC = new Vector3(0, 10000,0);
        Vector3 superTriangleD = new Vector3(0, 10000,10000);



        Tetraingulation.Add(new Tetrahederal(superTriangleA, superTriangleB, superTriangleC,superTriangleD));











        foreach (Vector3 point in pointList)
        {

            List<Tetrahederal> badTriangles = new List<Tetrahederal>();

            foreach (Tetrahederal tetra in Tetraingulation)
            {
                if (IspointInCircumsphere(tetra.A, tetra.B, tetra.C, tetra.B, point))
                {
                    badTriangles.Add(tetra);
                }
            }


            List<Triangle> polygon = new List<Triangle>();

            foreach (Tetrahederal tetra in badTriangles)
            {
                foreach (Triangle tri in tetra.triangles)
                {
                    foreach (Edge triangleEdge in tri.edges)
                    {
                        bool isShared = false;

                        foreach (Tetrahederal otherTetra in badTriangles)
                        {
                            if (otherTetra == tetra) { continue; }


                            foreach (var otherTri in otherTetra.triangles)
                            {
                                foreach (Edge otherEdge in otherTri.edges)
                                {
                                    if (LineIsEqual(triangleEdge, otherEdge))
                                    {
                                        isShared = true;
                                    }
                                }
                            }
                        }

                        if (isShared == false)
                        {
                            Vector3 alonevertex = Vector3.zero;   // get the alone vertex, with this i can get the dir of tringle so i can work out the perfect angle?????

                            if (triangleEdge.edge[0] != tri.a && triangleEdge.edge[1] != tri.a)
                            {
                                alonevertex = tri.a;
                            }
                            else if (triangleEdge.edge[0] != tri.b && triangleEdge.edge[1] != tri.b)
                            {
                                alonevertex = tri.b;
                            }
                            else if (triangleEdge.edge[0] != tri.c && triangleEdge.edge[1] != tri.c)
                            {
                                alonevertex = tri.c;
                            }

                            Debug.Log(alonevertex);

                            polygon.Add(new Triangle(alonevertex, triangleEdge.edge[0], triangleEdge.edge[1]));
                        }
                    }
                }
            }


            foreach (Tetrahederal badTriangle in badTriangles)
            {
                Tetraingulation.Remove(badTriangle);
            }



            // here you create a new tetra
            foreach (Triangle tri in polygon)
            {
                // this si for points
                Tetrahederal newTriangle = new Tetrahederal(tri.a, tri.b,tri.c, point);
                Tetraingulation.Add(newTriangle);
            }
        }



        for (int i = Tetraingulation.Count - 1; i >= 0; i--)
        {
            if (Tetraingulation[i].HasVertex(superTriangleA) || Tetraingulation[i].HasVertex(superTriangleB) || Tetraingulation[i].HasVertex(superTriangleC) || Tetraingulation[i].HasVertex(superTriangleD))
            {
                Tetraingulation.Remove(Tetraingulation[i]);
            }
        }

        Debug.Log(Tetraingulation.Count);


    }







    public class Tetrahederal 
    {

        public Triangle[] triangles = new Triangle[4];


        public Vector3 A = new Vector3();
        public Vector3 B = new Vector3();
        public Vector3 C = new Vector3();
        public Vector3 D = new Vector3();


        public Tetrahederal (Vector3 A, Vector3 B, Vector3 C, Vector3 D) 
        {

            this.A = A;
            this.D = D;
            this.B = B;
            this.C = C;



            triangles[0] = new Triangle(A, B, C);
            triangles[1] = new Triangle(A, B, D);
            triangles[2] = new Triangle(C, B, D);
            triangles[3] = new Triangle(C, A, D);
        }


        public bool HasVertex(Vector3 point)
        {

            if (point == A || point == B || point == C || point == D) { return true; }
            else { return false; }

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


    /// <summary>
    /// returns true if the point is in the circle    not needed
    /// </summary>
    //public bool IspointInCircumcircle(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    //{
    //    float ax_ = A[0] - D[0];
    //    float ay_ = A[1] - D[1];
    //    float bx_ = B[0] - D[0];
    //    float by_ = B[1] - D[1];
    //    float cx_ = C[0] - D[0];
    //    float cy_ = C[1] - D[1];

    //    if ((
    //        (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
    //        (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
    //        (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
    //    ) < 0)
    //    {
    //        return true;
    //    }

    //    else { return false; }
    //}


    public bool IspointInCircumsphere(Vector3 A, Vector3 B, Vector3 C, Vector3 D, Vector3 E)
    {

        Vector3 center =  Circumcentre(A,B,C,D);

        float radiusCenA = Vector3.Distance(center, A);
        float dist = Vector3.Distance(center, E);


        if (dist > radiusCenA) 
        {
            return false;
        }
        else 
        {
            return true;
        }


    }


    // to check
    public bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }









    //delete
    public Vector3 FindCircumcenter(Vector3 A, Vector3 B, Vector3 C, Vector3 D) 
    {
        Vector3 circumCenter = Vector3.zero;

        //a is the relative point

        double ba_x = B.x - A.x;
        double ba_y = B.y - A.y;
        double ba_z = B.z - A.z;

        double ca_x = C.x - A.x;
        double ca_y = C.y - A.y;
        double ca_z = C.z - A.z;

        double da_x = D.x - A.x;
        double da_y = D.y - A.y;
        double da_z = D.z - A.z;

        double len_ba = ba_x * ba_x + ba_y * ba_y + ba_z * ba_z;
        double len_ca = ca_x * ca_x + ca_y * ca_y + ca_z * ca_z;
        double len_da = da_x * da_x + da_y * da_y + da_z * da_z;



        // c cross d
        double cross_cd_x = ca_y * da_z - da_y * ca_z;
        double cross_cd_y = ca_z * da_x - da_z * ca_x;
        double cross_cd_z = ca_x * da_y - da_x * ca_y;

        // d cross b
        double cross_db_x = da_y * ba_z - ba_y * da_z;
        double cross_db_y = da_z * ba_x - ba_z * da_x;
        double cross_db_z = da_x * ba_y - ba_x * da_y;

        // b cross c
        double cross_bc_x = ba_y * ca_z - ca_y * ba_z;
        double cross_bc_y = ba_z * ca_x - ca_z * ba_x;
        double cross_bc_z = ba_x * ca_y - ca_x * ba_y;


        // Calculate the denominator of the formula.
        double denominator = 0.5 / (ba_x * cross_cd_x + ba_y * cross_cd_y + ba_z * cross_cd_z);


        double circ_x = (len_ba * cross_cd_x + len_ca * cross_db_x + len_da * cross_bc_x) * denominator;
        double circ_y = (len_ba * cross_cd_y + len_ca * cross_db_y + len_da * cross_bc_y) * denominator;
        double circ_z = (len_ba * cross_cd_z + len_ca * cross_db_z + len_da * cross_bc_z) * denominator;

        circumCenter.x = (float)circ_x *-1;
        circumCenter.y = (float)circ_y * -1;
        circumCenter.z = (float)circ_z;

        Debug.Log($"{circumCenter}");
        return circumCenter;

    }


}
