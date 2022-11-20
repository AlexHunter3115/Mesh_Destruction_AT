using static note;
using static UnityEngine.UI.Image;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine.UIElements;
using UnityEngine;

public class note : MonoBehaviour
{
    private bool edgeSet = false;
    private Vector3 edgeVertex = Vector3.zero;
    private Vector2 edgeUV = Vector2.zero;
    private Plane edgePlane = new Plane();

    public int CutCascades = 1;   // number of slices
    public float ExplodeForce = 0;    // explosive force, inbuilt unity function


    public GameObject planeCutter;
    public GameObject placeholder;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //DestroyMesh();
        }

        if (Input.GetMouseButtonDown(1))
        {



            Mesh planeMesh = planeCutter.transform.GetComponent<MeshFilter>().mesh;

            
            //Plane plane = new Plane(planeMesh.vertices[0], planeMesh.vertices[10], planeMesh.vertices[120]);


            Instantiate(placeholder, planeCutter.transform.TransformPoint(planeMesh.vertices[0]), placeholder.transform.rotation);    // top left 
            Instantiate(placeholder, planeCutter.transform.TransformPoint(planeMesh.vertices[110]), placeholder.transform.rotation);  //  top right 
            Instantiate(placeholder, planeCutter.transform.TransformPoint(planeMesh.vertices[120]), placeholder.transform.rotation);    //bot right
         
        }
    }








    private void DestroyMesh()
    {
        var originalMesh = GetComponent<MeshFilter>().mesh;
        originalMesh.RecalculateBounds();  // not important

        var parts = new List<PartMesh>();
        var subParts = new List<PartMesh>();


        // its compyinng the effected obj into a new class
        var mainPart = new PartMesh()
        {
            UV = originalMesh.uv,
            Vertices = originalMesh.vertices,
            Normals = originalMesh.normals,
            Triangles = new int[originalMesh.subMeshCount][],    // submeshes are the materails, so if a mesh had 2 mats they would have 2 submashes, this is making a 2d array for each submesh
            Bounds = originalMesh.bounds
        };

        // same as above
        for (int i = 0; i < originalMesh.subMeshCount; i++)
            mainPart.Triangles[i] = originalMesh.GetTriangles(i);




        // and then adds it to a part list
        parts.Add(mainPart);


        // cut cascadade should be the number of cuts
        for (var c = 0; c < CutCascades; c++)
        {
            //for every part, then creates subparts whichh then at the end beocme more parts 
            for (var i = 0; i < parts.Count; i++)
            {
                Bounds bounds = parts[i].Bounds; // bounds is simply aabb stuff 
                bounds.Expand(0.5f);   // this seems to half the bounds or somthing?


                // i think i can do without it and just use the plane in scene
                //var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                //                                                                   UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                //                                                                   UnityEngine.Random.Range(bounds.min.z, bounds.max.z)));

                Mesh planeMesh = planeCutter.transform.GetComponent<MeshFilter>().mesh;

                Plane plane = new Plane(planeCutter.transform.TransformPoint(planeMesh.vertices[0]),
                                        planeCutter.transform.TransformPoint(planeMesh.vertices[110]),
                                        planeCutter.transform.TransformPoint(planeMesh.vertices[120]) );


                subParts.Add(GenerateMesh(parts[i], plane, true));
                subParts.Add(GenerateMesh(parts[i], plane, false));
            }



            // set the subparts as the parts to then be slashed again
            parts = new List<PartMesh>(subParts);
            // clear the used list
            subParts.Clear();
        }










        //for every part created, the make object gives it all the componnents
        for (var i = 0; i < parts.Count; i++)
        {
            parts[i].MakeGameobject(this);
            // this gievs it the force
            parts[i].GameObject.GetComponent<Rigidbody>().AddForceAtPosition(parts[i].Bounds.center * ExplodeForce, transform.position);
        }

        // delete the copied object
        Destroy(gameObject);
    }









    // returns partmesh class type which is then used in the array to create the actual objects
    //given the original main part, the bool where it is cut and the      bool    
    private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
    {
        // new partmesh which is then going to be returned
        var partMesh = new PartMesh() { };

        // raycasting for the points of interestcion?
        var ray1 = new Ray();
        var ray2 = new Ray();

        //this is for every material remeber this is a 2d array and the first d is for the mat or sub mesh
        for (var i = 0; i < original.Triangles.Length; i++)
        {
            var triangles = original.Triangles[i];   // this is for the triangles in the sub mesh so the long sttrign of numns
            edgeSet = false;

            // the continue goes for the next iter of the loop
            for (var j = 0; j < triangles.Length; j = j + 3)
            {
                // oh ok this is an if statment, the get side is a built in (the plus or minus) , given the vector of the vertex with the is it equal to left?
                var sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                var sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                var sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                var sideCount = (sideA ? 1 : 0) + (sideB ? 1 : 0) + (sideC ? 1 : 0);  // this gets the answer from above, if true 1  else false, weeird style of if statment


                if (sideCount == 0)  // this 2 deal with the fact that if all on one side no point in continuing as no cutting is being done
                {
                    continue;
                }
                if (sideCount == 3)
                {
                    partMesh.AddTriangle(i,
                                         original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]], original.Vertices[triangles[j + 2]],
                                         original.Normals[triangles[j]], original.Normals[triangles[j + 1]], original.Normals[triangles[j + 2]],
                                         original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                    continue;
                }


                //condition ? consequent : alternative

                //cut points// i think this is tryint ot get the side from which the raycasts will come but this is awfull
                var singleIndex    =       sideB ==    sideC ? 0 : sideA         ==     sideC ? 1 : 2;

                // 


                ray1.origin = original.Vertices[triangles[j + singleIndex]];

                var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray1.direction = dir1;   // simlpe dir setting 

                // given a raycast tell the interaction, false if it goes through or not
                plane.Raycast(ray1, out var enter1);
                var lerp1 = enter1 / dir1.magnitude;

                //same thing
                ray2.origin = original.Vertices[triangles[j + singleIndex]];
                var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] - original.Vertices[triangles[j + singleIndex]];
                ray2.direction = dir2;
                plane.Raycast(ray2, out var enter2);
                var lerp2 = enter2 / dir2.magnitude;  // as far as i understand i shoot a ray cast from one vertex to another and when interesected get the out of 1 distance as magnitude, so 0.4 up the 1 big line example

                //lerp is a way to get along a line   0.5 lerp would be half way down a certain range of number
                //first vertex = ancor





                // might be able to cheta this bit after
                AddEdge(i,
                        partMesh,
                        left ? plane.normal * -1f : plane.normal,
                        ray1.origin + ray1.direction.normalized * enter1,
                        ray2.origin + ray2.direction.normalized * enter2,
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));




                // this makes sense depending on where the cut is in the relation add one or two tris
                if (sideCount == 1)
                {
                    partMesh.AddTriangle(i,
                                        original.Vertices[triangles[j + singleIndex]],
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        original.Normals[triangles[j + singleIndex]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        original.UV[triangles[j + singleIndex]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                    continue;
                }

                if (sideCount == 2)
                {
                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]]);




                    partMesh.AddTriangle(i,
                                        ray1.origin + ray1.direction.normalized * enter1,
                                        original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                                        ray2.origin + ray2.direction.normalized * enter2,
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector3.Lerp(original.Normals[triangles[j + singleIndex]], original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                                        original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                                        Vector2.Lerp(original.UV[triangles[j + singleIndex]], original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                    continue;
                }

            }
        }

        partMesh.FillArrays();

        return partMesh;
    }






    private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1, Vector2 uv2)
    {
        if (!edgeSet)
        {
            edgeSet = true;
            edgeVertex = vertex1;
            edgeUV = uv1;
        }
        else
        {
            edgePlane.Set3Points(edgeVertex, vertex1, vertex2);

            partMesh.AddTriangle(subMesh,
                                edgeVertex,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                                edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,
                                normal,
                                normal,
                                normal,
                                edgeUV,
                                uv1,
                                uv2);
        }
    }







    // this is the part thats going to be constuctde
    public class PartMesh
    {
        private List<Vector3> _Verticies = new List<Vector3>();
        private List<Vector3> _Normals = new List<Vector3>();
        private List<List<int>> _Triangles = new List<List<int>>();
        private List<Vector2> _UVs = new List<Vector2>();
        public Vector3[] Vertices;
        public Vector3[] Normals;


        /// <summary>
        /// given the submesh (material index) returns all the tris in that mats as the 2nd deminsion
        /// </summary>
        public int[][] Triangles;

        public Vector2[] UV;
        public GameObject GameObject;

        //still nto too sure what the bounds actually do
        public Bounds Bounds = new Bounds();

        public PartMesh()
        {

        }

        public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1, Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            if (_Triangles.Count - 1 < submesh)
                _Triangles.Add(new List<int>());

            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert1);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert2);
            _Triangles[submesh].Add(_Verticies.Count);
            _Verticies.Add(vert3);
            _Normals.Add(normal1);
            _Normals.Add(normal2);
            _Normals.Add(normal3);
            _UVs.Add(uv1);
            _UVs.Add(uv2);
            _UVs.Add(uv3);

            Bounds.min = Vector3.Min(Bounds.min, vert1);
            Bounds.min = Vector3.Min(Bounds.min, vert2);
            Bounds.min = Vector3.Min(Bounds.min, vert3);
            Bounds.max = Vector3.Min(Bounds.max, vert1);
            Bounds.max = Vector3.Min(Bounds.max, vert2);
            Bounds.max = Vector3.Min(Bounds.max, vert3);
        }

        public void FillArrays()
        {
            Vertices = _Verticies.ToArray();
            Normals = _Normals.ToArray();
            UV = _UVs.ToArray();
            Triangles = new int[_Triangles.Count][];
            for (var i = 0; i < _Triangles.Count; i++)
                Triangles[i] = _Triangles[i].ToArray();
        }

        public void MakeGameobject(note original)
        {
            GameObject = new GameObject(original.name);
            GameObject.transform.position = original.transform.position;
            GameObject.transform.rotation = original.transform.rotation;
            GameObject.transform.localScale = original.transform.localScale;

            var mesh = new Mesh();
            mesh.name = original.GetComponent<MeshFilter>().mesh.name;

            mesh.vertices = Vertices;
            mesh.normals = Normals;
            mesh.uv = UV;
            for (var i = 0; i < Triangles.Length; i++)
                mesh.SetTriangles(Triangles[i], i, true);
            Bounds = mesh.bounds;

            var renderer = GameObject.AddComponent<MeshRenderer>();
            renderer.materials = original.GetComponent<MeshRenderer>().materials;

            var filter = GameObject.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var collider = GameObject.AddComponent<MeshCollider>();
            collider.convex = true;

            var rigidbody = GameObject.AddComponent<Rigidbody>();
            var meshDestroy = GameObject.AddComponent<note>();
            meshDestroy.CutCascades = original.CutCascades;
            meshDestroy.ExplodeForce = original.ExplodeForce;
            meshDestroy.planeCutter = original.planeCutter;
            meshDestroy.placeholder = original.placeholder;
        }

    }
}