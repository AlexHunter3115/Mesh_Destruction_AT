using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelColOnCol : MonoBehaviour
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

        Destroy(this.GetComponent<MeshCollider>());

        yield return new WaitForSeconds(dissapearTimer);

        Destroy(gameObject);
    }


}
