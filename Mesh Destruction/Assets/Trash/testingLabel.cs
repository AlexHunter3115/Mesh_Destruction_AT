using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class testingLabel : MonoBehaviour
{

    public bool local = false;
    void OnDrawGizmos()
    {
        if (local)
            Handles.Label(transform.position, $"{transform.position}");
        else
            Handles.Label(transform.position, $"{transform.localPosition}");
    }
}
