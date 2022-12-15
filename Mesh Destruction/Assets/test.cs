using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    void OnMouseDown()
    {
        // this object was clicked - do something
        Destroy(this.gameObject);
    }
}
