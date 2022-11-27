using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVertexData : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = this.transform.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        List<Vector3> verticesToSend = new List<Vector3>();

        for (var i = 0; i < vertices.Length; i++)
        {
            if (!verticesToSend.Contains(vertices[i])) 
            {
                verticesToSend.Add(vertices[i]);
            }
        }
        //Debug.Log($"{mesh.GetTriangles(0)[0]}");
        //Debug.Log($"{mesh.GetTriangles(0)[1]}");
       // Debug.Log($"{mesh.GetTriangles(3)[2]}");



        //Debug.Log($"{mesh.triangles(0)[2]}");



        List<int> tris = new List<int>();



        //for (var i = 0; i < mesh.triangles.Length; i++)
        //{
        //    Debug.Log($"{mesh.triangles[i]}"); ;
        //}

        //Debug.Log($"eroweqewoipewqioqweoipqewoip");

        //for (var i = 0; i < mesh.vertices.Length; i++)
        //{
        //    Debug.Log($"{mesh.vertices[i]}");
        //}


        //MeshGenerator.Instance.UpdateMesh(mesh);


    }

    
}
