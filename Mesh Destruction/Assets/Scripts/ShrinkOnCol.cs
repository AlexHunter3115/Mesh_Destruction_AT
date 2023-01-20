using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkOnCol : MonoBehaviour
{
    public float dissapearTimer = 2;
    private bool shrink = false;

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(MyCoroutine());
    }


    private IEnumerator MyCoroutine()
    {
        yield return new WaitForSeconds(dissapearTimer);

        shrink = true;
    }


    private void Update()
    {
        if (shrink) 
        {
            if (transform.localScale.x <= 0) 
            {
                Destroy(gameObject);
            }
            else 
            {
                transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
    }


}
