using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float destoryTimer;

    void Start()
    {
        Destroy(this.gameObject, destoryTimer);        
    }
}
